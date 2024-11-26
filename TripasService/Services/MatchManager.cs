﻿using DataBaseManager.DAO;
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
            if (!activeMatches.TryRemove(matchCode, out var match)) {
                Console.WriteLine($"La partida {matchCode} no existe o ya fue eliminada.");
                return false;
            }

            foreach (var player in match.Players.Values) {
                if (player != null) {
                    int finalScore = match.GetPlayerScore(player.Username);
                    UserDAO.UpdatePlayerScore(player.Username, finalScore);
                    Console.WriteLine($"Jugador {player.Username} recibió {finalScore} puntos. Guardados en la BD.");
                }
            }

            foreach (var player in match.Players.Values) {
                if (player != null && matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    NotifyPlayerMatchEnded(player.Username, callback);
                }
            }
            Console.WriteLine($"La partida {matchCode} ha terminado exitosamente.");
            return true;
        }

        private void NotifyPlayerMatchEnded(string playerName, IMatchManagerCallback callback) {
            try {
                callback.NotifyMatchEnded();
                Console.WriteLine($"Notificado al jugador {playerName} del fin de la partida.");
            } catch (CommunicationException communicationException) {
                Console.WriteLine($"Error al notificar a {playerName}: {communicationException.Message}");
                matchPlayerCallback.TryRemove(playerName, out _);
            } catch (TimeoutException timeoutException) {
                Console.WriteLine($"Error de timeOut notificando a {playerName}: {timeoutException.Message}");
                matchPlayerCallback.TryRemove(playerName, out _);
            } catch (Exception ex) {
                Console.WriteLine($"Error inesperado notificando a {playerName}: {ex.Message}");
            }
        }

        public GameResult GetGameResult(string matchCode, string username) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"Partida con código {matchCode} no encontrada.");
                return null; // O lanzar una excepción
            }

            var playersScores = match.CurrentScores;

            if (!playersScores.ContainsKey(username)) {
                Console.WriteLine($"El jugador {username} no participa en la partida {matchCode}.");
                return null; // O lanzar una excepción
            }

            var highestScore = playersScores.Values.Max();
            var winners = playersScores
                .Where(x => x.Value == highestScore)
                .Select(x => x.Key)
                .ToList();

            return new GameResult {
                IsWinner = winners.Contains(username),
                IsDraw = winners.Count > 1,
                Score = playersScores[username]
            };
        }
    }
}