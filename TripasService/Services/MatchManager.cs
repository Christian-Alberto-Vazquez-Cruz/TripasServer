using DataBaseManager.DAO;
using TripasService.Utils;
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

        private static readonly ConcurrentDictionary<string, Match> _activeMatches = new ConcurrentDictionary<string, Match>();
        private static readonly ConcurrentDictionary<string, IMatchManagerCallback> _matchPlayerCallback = new ConcurrentDictionary<string, IMatchManagerCallback>();
        public List<Node> GetNodes(string matchCode) {
            //INICIALIZAR UNA LISTA VACÍA. EN EL CLIENTE PREGUNTAR SI HAY MÁS DE 0 ITEMS
            if (!_activeMatches.TryGetValue(matchCode, out var match)) return null;
            return match.GetAllNodes();
        }

        public Dictionary<string, string> GetNodePairs(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) return null;
            return match.GetNodePairs();
        }

        public bool RegisterPlayerCallback(string matchCode, string username) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) return false;

            var callback = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            if (match.Players.Values.Any(player => player.Username == username)) {
                return _matchPlayerCallback.TryAdd(username, callback);
            }
            Console.WriteLine($"El jugador {username} no pertenece a la partida {matchCode}.");
            return false;
        }

        public bool EndTurn(string matchCode, string userName) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"Partida {matchCode} no encontrada.");
                return false;
            }

            match.SwitchTurn();
            Console.WriteLine($"Cambio de turno en la partida {matchCode}. Turno actual: {match.CurrentTurn}.");

            foreach (var player in match.Players.Values) {
                if (player != null && _matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        if (player.Username == match.CurrentTurn) {
                            callback.NotifyYourTurn();
                        } else {
                            callback.NotifyNotYourTurn();
                        }
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al notificar al jugador {player.Username}: {ex.Message}");
                        _matchPlayerCallback.TryRemove(player.Username, out _);
                    }
                }
            }
            return true;
        }

        public bool RegisterTrace(string matchCode, Trace trace) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) return false;
            match.AddTrace(trace);

            int tracePoints = trace.Score;
            match.AddPoints(trace.Player, tracePoints);

            Console.WriteLine($"Jugador {trace.Player} recibió {tracePoints} puntos en la partida {matchCode}. Total actual: {match.GetPlayerScore(trace.Player)}");

            foreach (var player in match.Players.Values) {
                if (player.Username != trace.Player && _matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        callback.TraceReceived(trace);
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al notificar al jugador {player.Username}: {ex.Message}");
                        _matchPlayerCallback.TryRemove(player.Username, out _);
                    }
                }
            }
            return true;
        }

        //REGRESAR TURNO STRING VACÍO?
        public string GetCurrentTurn(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"Partida {matchCode} no encontrada.");
                return null;  // O devolver un mensaje de error o valor especial si no se encuentra la partida
            }
            return match.CurrentTurn;
        }


        public void EndMatch(string matchCode) {
            Match match = GetMatch(matchCode);
            if (match.Code == Constants.NO_MATCHES_STRING) {
                return;
            }

            SavePlayerScores(match);
            NotifyMatchResults(match);
            RemoveMatchCallbacks(matchCode);
        }

        private void SavePlayerScores(Match match) {
            foreach (var player in match.Players.Values.Where(player => player.IdProfile < Constants.MIN_ID_GUEST ||
            player.IdProfile > Constants.MAX_ID_GUEST)) {
                int finalScore = match.GetPlayerScore(player.Username);
                UserDAO.UpdatePlayerScore(player.Username, finalScore);
            }
        }

        private void NotifyMatchResults(Match match) {
            var scores = match.CurrentScores;
            var highestScore = scores.Values.Max();
            var winners = scores.Where(x => x.Value == highestScore).Select(x => x.Key).ToList();

            foreach (var player in match.Players.Values.Where(p => p != null)) {
                string username = player.Username;
                NotifyPlayerResult(username, winners, match.Code);
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
            if (_matchPlayerCallback.TryGetValue(userName, out var callback)) {
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
                _matchPlayerCallback.TryRemove(userName, out _);
                Console.WriteLine($"Callback removed for {userName} due to communication error.");
            }
            return false;
        }

        public void LeaveMatch(string matchCode, string username) {

            Match match = GetMatch(matchCode);
            if (match.Code == Constants.NO_MATCHES_STRING) {
                return;
            }

            if (!match.Players.Values.Any(player => player != null && player.Username == username)) {
                Console.WriteLine($"El jugador {username} no pertenece a la partida {matchCode}.");
                return;
            }

            var leavingPlayerKey = match.Players.FirstOrDefault(p => p.Value?.Username == username).Key;
            var opponent = match.Players.Values.FirstOrDefault(p => p != null && p.Username != username);

            if (opponent != null && _matchPlayerCallback.TryGetValue(opponent.Username, out var callback)) {
                try {
                    callback.NotifyPlayerLeft();
                    Console.WriteLine($"El jugador {opponent.Username} ha sido notificado de que debe abandonar la partida {matchCode}.");
                } catch (Exception ex) {
                    Console.WriteLine($"Error al notificar al jugador {opponent.Username}: {ex.Message}");
                    _matchPlayerCallback.TryRemove(opponent.Username, out _);
                }
            }

            RemoveMatchCallbacks(matchCode);
            EndMatchByAbandonment(matchCode);
        }

        private void EndMatchByAbandonment(string matchCode) {
            if (!_activeMatches.TryRemove(matchCode, out var match)) {
                Console.WriteLine($"La partida {matchCode} ya había sido eliminada.");
                return;
            }

            Console.WriteLine($"La partida {matchCode} ha sido eliminada debido al abandono.");
            SavePlayerScores(match); // Registrar los puntajes si es necesario
        }

        private void RemoveMatchCallbacks(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"La partida {matchCode} no existe o ya fue eliminada.");
                return;
            }

            foreach (var player in match.Players.Values.Where(p => p != null)) {
                if (_matchPlayerCallback.TryRemove(player.Username, out _)) {
                    Console.WriteLine($"El callback del jugador {player.Username} ha sido eliminado de la partida {matchCode}.");
                } else {
                    Console.WriteLine($"No se encontró un callback asociado al jugador {player.Username} en la partida {matchCode}.");
                }
            }
        }

        private Match GetMatch(string matchCode) {
            Match match = new Match() {
                Code = Constants.NO_MATCHES_STRING
            };

            if (_activeMatches.TryGetValue(matchCode, out var matchSearched)) {
                match = matchSearched;
            }
            return match;
        }
    }
}
