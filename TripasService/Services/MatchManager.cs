﻿using System;
using System.Linq;
using TripasService.Utils;
using DataBaseManager.DAO;
using TripasService.Logic;
using System.ServiceModel;
using TripasService.Contracts;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace TripasService.Services {

    public partial class TripasGameService : IMatchManager {

        private static readonly ConcurrentDictionary<string, Match> _activeMatches = new ConcurrentDictionary<string, Match>();
        private static readonly ConcurrentDictionary<string, IMatchManagerCallback> _matchPlayerCallback = new ConcurrentDictionary<string, IMatchManagerCallback>();

        public List<Node> GetNodes(string matchCode) {
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
            LoggerManager logger = new LoggerManager(this.GetType());
            if (!_activeMatches.TryGetValue(matchCode, out var match)) {
                return false;
            }
            match.SwitchTurn();
            foreach (var player in match.Players.Values) {
                if (player != null && _matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        if (player.Username == match.CurrentTurn) {
                            callback.NotifyYourTurn();
                        } else {
                            callback.NotifyNotYourTurn();
                        }
                    } catch (Exception exception) {
                        logger.LogError($"Error al notificar al jugador {player.Username}: {exception.Message}", exception);
                        LeaveMatch(matchCode, userName);
                    }
                }
            }
            return true;
        }


        public bool RegisterTrace(string matchCode, Trace trace) {
            LoggerManager logger = new LoggerManager(this.GetType());
            if (!_activeMatches.TryGetValue(matchCode, out var match)) return false;
            match.AddTrace(trace);
            int tracePoints = trace.Score;
            match.AddPoints(trace.Player, tracePoints);
            Console.WriteLine($"Jugador {trace.Player} recibió {tracePoints} puntos en la partida {matchCode}. Total actual: {match.GetPlayerScore(trace.Player)}");
            foreach (var player in match.Players.Values) {
                if (player.Username != trace.Player && _matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        callback.TraceReceived(trace);
                    } catch (Exception exception) {
                        logger.LogError($"Error al notificar al jugador {player.Username}: {exception.Message}", exception);
                        _matchPlayerCallback.TryRemove(player.Username, out _);
                    }
                }
            }
            return true;
        }

        public string GetCurrentTurn(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out var match)) {
                Console.WriteLine($"Partida {matchCode} no encontrada.");
                return null;  
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
            LoggerManager logger = new LoggerManager(this.GetType());
            if (_matchPlayerCallback.TryGetValue(userName, out var callback)) {
                try {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
                        callbackAction(callback);
                        return true;
                    }
                } catch (CommunicationException communicationException) {
                    logger.LogError($"Communication error with {userName}: {communicationException.Message}", communicationException);
                } catch (TimeoutException timeoutException) {
                    logger.LogError($"Timeout while notifying {userName}: {timeoutException.Message}", timeoutException);
                } catch (ObjectDisposedException objectDisposedException) {
                    logger.LogError($"Channel was disposed for {userName}: {objectDisposedException.Message}", objectDisposedException);
                } catch (Exception exception) {
                    logger.LogError($"Unexpected error notifying {userName}: {exception.Message}", exception);
                }
                _matchPlayerCallback.TryRemove(userName, out _);
                Console.WriteLine($"Callback removed for {userName} due to communication error.");
            }
            return false;
        }

        public void LeaveMatch(string matchCode, string username) {
            LoggerManager logger = new LoggerManager(this.GetType());
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
                } catch (Exception exception) {
                    logger.LogError($"Error notifying player {opponent.Username} about departure: {exception.Message}", exception);
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
            SavePlayerScores(match); 
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