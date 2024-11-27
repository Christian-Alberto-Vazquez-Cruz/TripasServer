using DataBaseManager.DAO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using TripasService.Logic;

namespace TripasService.Services {
    public partial class TripasGameService : IMatchManager {

        private static ConcurrentDictionary<string, Match> activeMatches = new ConcurrentDictionary<string, Match>();
        private static ConcurrentDictionary<string, IMatchManagerCallback> matchPlayerCallback = new ConcurrentDictionary<string, IMatchManagerCallback>();
        public List<Node> GetNodes(string matchCode) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return null;
            return match.GetAllNodes();
        }

        public Dictionary<string, string> GetNodePairs(string matchCode) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return null;
            return match.GetNodePairs();
        }

        public bool RegisterPlayerCallback(string matchCode, string username) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return false;

            var callback = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            if (match.Players.Values.Any(player => player.Username == username)) {
                return matchPlayerCallback.TryAdd(username, callback);
            }

            Console.WriteLine($"El jugador {username} no pertenece a la partida {matchCode}.");
            return false;
        }

        public bool EndTurn(string matchCode, string userName) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"Partida {matchCode} no encontrada.");
                return false;
            }

            // Cambiar turno
            match.SwitchTurn();
            Console.WriteLine($"Cambio de turno en la partida {matchCode}. Turno actual: {match.CurrentTurn}.");

            foreach (var player in match.Players.Values) {
                if (player != null && matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        if (player.Username == match.CurrentTurn) {
                            callback.NotifyYourTurn();
                        } else {
                            callback.NotifyNotYouTurn();
                        }
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al notificar al jugador {player.Username}: {ex.Message}");
                        matchPlayerCallback.TryRemove(player.Username, out _);
                    }
                }
            }

            return true;
        }

        public bool RegisterTrace(string matchCode, Trace trace) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return false;
            match.AddTrace(trace);

            int tracePoints = trace.Score;
            match.AddPoints(trace.Player, tracePoints);

            Console.WriteLine($"Jugador {trace.Player} recibió {tracePoints} puntos en la partida {matchCode}. Total actual: {match.GetPlayerScore(trace.Player)}");

            foreach (var player in match.Players.Values) {
                if (player.Username != trace.Player && matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        callback.TraceReceived(trace);
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al notificar al jugador {player.Username}: {ex.Message}");
                        matchPlayerCallback.TryRemove(player.Username, out _);
                    }
                }
            }

            return true;
        }

        // Método en el servidor para obtener el turno actual de la partida
        public string GetCurrentTurn(string matchCode) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"Partida {matchCode} no encontrada.");
                return null;  // O devolver un mensaje de error o valor especial si no se encuentra la partida
            }

            return match.CurrentTurn;
        }


        public bool EndMatch(string matchCode) {
            var match = GetAndRemoveMatch(matchCode);
            if (match == null) return false;

            SavePlayerScores(match);
            NotifyMatchResults(match, matchCode);

            Console.WriteLine($"Match {matchCode} has ended successfully.");
            return true;
        }

        private Match GetAndRemoveMatch(string matchCode) {
            if (!activeMatches.TryRemove(matchCode, out var match)) {
                Console.WriteLine($"The match {matchCode} does not exist or has already been removed.");
                return null;
            }
            return match;
        }
        private void SavePlayerScores(Match match) {
            foreach (var player in match.Players.Values.Where(player => player != null)) {
                int finalScore = match.GetPlayerScore(player.Username);
                UserDAO.UpdatePlayerScore(player.Username, finalScore);
                Console.WriteLine($"Player {player.Username} scored {finalScore} points. Saved to the database.");
            }
        }

        private void NotifyMatchResults(Match match, string matchCode) {
            var scores = match.CurrentScores;
            var highestScore = scores.Values.Max();
            var winners = scores.Where(x => x.Value == highestScore).Select(x => x.Key).ToList();

            foreach (var player in match.Players.Values.Where(p => p != null)) {
                string username = player.Username;
                NotifyPlayerResult(username, winners, matchCode);
            }
        }

        private void NotifyPlayerResult(string username, List<string> winners, string matchCode) {
            if (winners.Contains(username)) {
                if (winners.Count > 1) {
                    TryNotifyMatchCallback(username, callback => callback.NotifyDraw());
                    Console.WriteLine($"Player {username} notified of a draw in match {matchCode}.");
                } else {
                    TryNotifyMatchCallback(username, callback => callback.NotifyYouWon());
                    Console.WriteLine($"Player {username} won match {matchCode}.");
                }
            } else {
                TryNotifyMatchCallback(username, callback => callback.NotifyYouLost());
                Console.WriteLine($"Player {username} lost match {matchCode}.");
            }
        }

        private bool TryNotifyMatchCallback(string userName, Action<IMatchManagerCallback> callbackAction) {
            if (matchPlayerCallback.TryGetValue(userName, out var callback)) {
                try {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
                        callbackAction(callback);
                        return true;
                    }
                } catch (CommunicationException ex) {
                    Console.WriteLine($"Communication error with {userName}: {ex.Message}");
                } catch (TimeoutException ex) {
                    Console.WriteLine($"Timeout while notifying {userName}: {ex.Message}");
                } catch (ObjectDisposedException ex) {
                    Console.WriteLine($"Channel was disposed for {userName}: {ex.Message}");
                } catch (Exception ex) {
                    Console.WriteLine($"Unexpected error notifying {userName}: {ex.Message}");
                }
                matchPlayerCallback.TryRemove(userName, out _);
                Console.WriteLine($"Callback removed for {userName} due to communication error.");
            }
            return false;
        }
    }
}