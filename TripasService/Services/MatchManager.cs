using System;
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

        public Dictionary<string, string> GetNodePairs(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) return null;
            return match.GetNodePairs();
        }

        public bool RegisterPlayerCallback(string matchCode, string username) {
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) return false;
            var callback = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();
            if (match.Players.Values.Any(player => player.Username == username)) {
                return _matchPlayerCallback.TryAdd(username, callback);
            }
            return false;
        }

        public bool EndTurn(string matchCode, string userName) {
            LoggerManager logger = new LoggerManager(this.GetType());
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) {
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
                    } catch (CommunicationException communicationException) {
                        logger.LogError($"Error trying to notify {player.Username}: {communicationException.Message}", communicationException);
                    } catch (TimeoutException timeoutException) {
                        logger.LogError($"Error trying to notify {player.Username}: {timeoutException.Message}", timeoutException);
                    } catch (Exception exception) {
                        logger.LogError($"Error trying to notify {player.Username}: {exception.Message}", exception);
                    }
                }
            }
            return true;
        }


        public bool RegisterTrace(string matchCode, Trace trace) {
            LoggerManager logger = new LoggerManager(this.GetType());
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) {
                return false;
            }

            int tracePoints = trace.Score;
            match.AddPoints(trace.Player, tracePoints);
            foreach (var player in match.Players.Values) {
                if (player.Username != trace.Player && _matchPlayerCallback.TryGetValue(player.Username, out var callback)) {
                    try {
                        callback.TraceReceived(trace);
                    } catch (CommunicationException communicationException) {
                        logger.LogError($"Error al notificar al jugador {player.Username}: {communicationException.Message}", communicationException);
                        LeaveMatch(matchCode, player.Username);
                        _matchPlayerCallback.TryRemove(player.Username, out _);
                    } catch (TimeoutException timeoutException) {
                        logger.LogError($"Error al notificar al jugador {player.Username}: {timeoutException.Message}", timeoutException);
                        LeaveMatch(matchCode, player.Username);
                        _matchPlayerCallback.TryRemove(player.Username, out _);
                    } catch (Exception exception) {
                        logger.LogError($"Error al notificar al jugador {player.Username}: {exception.Message}", exception);
                        LeaveMatch(matchCode, player.Username);
                        _matchPlayerCallback.TryRemove(player.Username, out _);
                    }
                }
            }
            return true;
        }


        public string GetCurrentTurn(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) {
                return null;  
            }
            return match.CurrentTurn;
        }
        public List<Node> GetNodes(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) {
                return null;
            }
            return match.GetAllNodes();
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
            var winners = scores.Where(playerScore => playerScore.Value == highestScore).Select(player => player.Key).ToList();
            foreach (Profile player in match.Players.Values.Where(player => player != null)) {
                string username = player.Username;
                NotifyPlayerResult(username, winners, match.Code);
            }
        }

        private void NotifyPlayerResult(string username, List<string> winners, string matchCode) {
            if (winners.Contains(username)) {
                if (winners.Count > 1) {
                    TryNotifyMatchCallback(username, callback => callback.NotifyDraw());
                } else {
                    TryNotifyMatchCallback(username, callback => callback.NotifyYouWon());
                }
            } else {
                TryNotifyMatchCallback(username, callback => callback.NotifyYouLost());
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
                return;
            }
            var leavingPlayerKey = match.Players.FirstOrDefault(player => player.Value?.Username == username).Key;
            var opponent = match.Players.Values.FirstOrDefault(player => player != null && player.Username != username);
            if (opponent != null && _matchPlayerCallback.TryGetValue(opponent.Username, out var callback)) {
                try {
                    callback.NotifyPlayerLeft();
                } catch (CommunicationException communicationException) {
                    logger.LogError($"Error notifying player {opponent.Username} about departure: {communicationException.Message}", communicationException);
                    _matchPlayerCallback.TryRemove(opponent.Username, out _);
                } catch (TimeoutException timeoutException) {
                    logger.LogError($"Error notifying player {opponent.Username} about departure: {timeoutException.Message}", timeoutException);
                    _matchPlayerCallback.TryRemove(opponent.Username, out _);
                } catch (Exception exception) {
                    logger.LogError($"Error notifying player {opponent.Username} about departure: {exception.Message}", exception);
                    _matchPlayerCallback.TryRemove(opponent.Username, out _););
                }
            }
            RemoveMatchCallbacks(matchCode);
            EndMatchByAbandonment(matchCode);
        }



        private void EndMatchByAbandonment(string matchCode) {
            if (!_activeMatches.TryRemove(matchCode, out Match match)) {
                return;
            }
            SavePlayerScores(match); 
        }

        private void RemoveMatchCallbacks(string matchCode) {
            if (!_activeMatches.TryGetValue(matchCode, out Match match)) {
                return;
            }
            foreach (var player in match.Players.Values.Where(player => player != null)) {
                _matchPlayerCallback.TryRemove(player.Username, out _); 
            }
        }

        private Match GetMatch(string matchCode) {
            Match match = new Match() {
                Code = Constants.NO_MATCHES_STRING
            };
            if (_activeMatches.TryGetValue(matchCode, out Match matchSearched)) {
                match = matchSearched;
            }
            return match;
        }
    }
}