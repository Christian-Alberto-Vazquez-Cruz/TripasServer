using System;
using System.ServiceModel;
using TripasService.Logic;
using TripasService.Contracts;
using System.Collections.Generic;
using System.Collections.Concurrent;
using TripasService.Utils;
using log4net.Repository.Hierarchy;

namespace TripasService.Services {
    public partial class TripasGameService : ILobbyManager {
        private static readonly ConcurrentDictionary<string, Lobby> _lobbies = new ConcurrentDictionary<string, Lobby>();
        private static readonly ConcurrentDictionary<string, ILobbyManagerCallback> _lobbyPlayerCallback = new ConcurrentDictionary<string, ILobbyManagerCallback>();

        private bool TryNotifyCallback(string username, Action<ILobbyManagerCallback> callbackAction) {
            LoggerManager logger = new LoggerManager(this.GetType());
            if (_lobbyPlayerCallback.TryGetValue(username, out var callback)) {
                try {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
                        callbackAction(callback);
                        return true;
                    }
                } catch (CommunicationException communicationException) {
                    logger.LogError($"Communication error with {username}: {communicationException.Message}", communicationException);
                } catch (TimeoutException timeoutException) {
                    logger.LogError($"Timeout while notifying {username}: {timeoutException.Message}", timeoutException);
                } catch (ObjectDisposedException objectDisposedException) {
                    logger.LogError($"Channel was disposed for {username}: {objectDisposedException.Message}", objectDisposedException);
                }
                _lobbyPlayerCallback.TryRemove(username, out _);
            }
            return false;
        }

        public bool ConnectPlayerToLobby(string code, int playerId) {
            var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            bool operationResult = false;
            if (!_lobbies.TryGetValue(code, out Lobby lobby)) {
                return operationResult;
            }
            if (TryRegisterCallbackForHost(lobby, playerId, callback) || TryRegisterCallbackForGuest(lobby, playerId, callback)) {
                operationResult = true;
            }
            return operationResult;
        }
        public void LeaveLobby(string code, string username) {
            if (!TryGetLobby(code, out Lobby lobby)) return;

            if (IsPlayerHost(lobby, username, out Profile host)) {
                HandleHostLeaving(lobby, code, host);
            } else if (IsPlayerGuest(lobby, username, out Profile guest)) {
                HandleGuestLeaving(lobby, guest);
            }
        }

        private bool IsPlayerHost(Lobby lobby, string username, out Profile host) {
            if (lobby.Players.TryGetValue("PlayerOne", out host) && host.Username == username) {
                return true;
            }
            return false;
        }

        private bool IsPlayerGuest(Lobby lobby, string username, out Profile guest) {
            if (lobby.Players.TryGetValue("PlayerTwo", out guest) && guest.Username == username) {
                return true;
            }
            return false;
        }

        private void HandleHostLeaving(Lobby lobby, string code, Profile host) {
            _lobbyPlayerCallback.TryRemove(host.Username, out _);
            NotifyGuestOfHostDisconnection(lobby);
            DeleteLobby(code);
        }

        private void HandleGuestLeaving(Lobby lobby, Profile guest) {
            lobby.Players.Remove("PlayerTwo");
            _lobbyPlayerCallback.TryRemove(guest.Username, out _);
            if (lobby.Players.TryGetValue("PlayerOne", out Profile host)) {
                TryNotifyCallback(host.Username, callback => callback.GuestLeftCallback());
            }
        }

        private void NotifyGuestOfHostDisconnection(Lobby lobby) {
            if (lobby.Players.TryGetValue("PlayerTwo", out Profile guest) && guest != null) {
                TryNotifyCallback(guest.Username, callback => callback.HostLeftCallback());
                _lobbyPlayerCallback.TryRemove(guest.Username, out _);
            }
        }

        private bool TryRegisterCallbackForHost(Lobby lobby, int playerId, ILobbyManagerCallback callback) {
            if (lobby.Players.TryGetValue("PlayerOne", out Profile host) && host.IdProfile == playerId) {
                if (_lobbyPlayerCallback.TryAdd(host.Username, callback)) {
                    return true;
                }
            }
            return false;
        }

        private bool TryRegisterCallbackForGuest(Lobby lobby, int playerId, ILobbyManagerCallback callback) {
            if (lobby.Players.TryGetValue("PlayerTwo", out Profile guest) && guest.IdProfile == playerId) {
                if (_lobbyPlayerCallback.TryAdd(guest.Username, callback)) {
                    NotifyHostAboutGuest(lobby, guest);
                    return true;
                }
            }
            return false;
        }

        private void NotifyHostAboutGuest(Lobby lobby, Profile guest) {
            string hostUsername = GetPlayer(lobby, "PlayerOne");
            if (!string.IsNullOrEmpty(hostUsername)) {
                TryNotifyCallback(hostUsername, callback => callback.GuestJoinedCallback(guest.Username, guest.PicturePath, guest.IdProfile));
            }
        }

        public void StartMatch(string code) {
            if (!TryGetLobby(code, out Lobby lobby)) {
                return;
            }
            if (!TryGetHost(lobby, out Profile host) || !TryGetGuest(lobby, out Profile guest)) {
                return;
            }
            Match match = CreateMatch(code, lobby, host, guest);
            match.StartGame();
            if (!TryAddMatchToActiveMatches(code, match)) {
                return;
            }
            NotifyPlayersMatchStarted(host, guest);
            RemoveLobbyCallbacks(code);
            RemoveChatCallbacks(code);
        }

        private Match CreateMatch(string code, Lobby lobby, Profile host, Profile guest) {
            return new Match(
              code,
              lobby.GameName,
              lobby.NodeCount,
              new Dictionary<string, Profile>
              {
                { "PlayerOne", host },
                { "PlayerTwo", guest }
              }
          );
        }

        private bool TryGetLobby(string code, out Lobby lobby) {
            if (!_lobbies.TryGetValue(code, out lobby)) {
                return false;
            }
            return true;
        }

        private bool TryGetHost(Lobby lobby, out Profile host) {
            if (!lobby.Players.TryGetValue("PlayerOne", out host)) {
                return false;
            }
            return true;
        }

        private bool TryGetGuest(Lobby lobby, out Profile guest) {
            if (!lobby.Players.TryGetValue("PlayerTwo", out guest)) {
                return false;
            }
            return true;
        }

        private bool TryAddMatchToActiveMatches(string code, Match match) {
            if (!_activeMatches.TryAdd(code, match)) {
                return false;
            }
            return true;
        }


        private void NotifyPlayersMatchStarted(Profile host, Profile guest) {
            TryNotifyCallback(host.Username, cb => cb.GameStarted());
            TryNotifyCallback(guest.Username, cb => cb.GameStarted());
        }

        private void RemoveLobbyCallbacks(string code) {
            LoggerManager logger = new LoggerManager(this.GetType());
            if (_lobbies.TryGetValue(code, out var lobby)) {
                foreach (Profile player in lobby.Players.Values) {
                    _lobbyPlayerCallback.TryRemove(player.Username, out _);
                }
                DeleteLobby(code);
            } 
        }

        public bool DeleteLobby(string code) {
            bool operationResult = _lobbies.TryRemove(code, out _);
            return operationResult;
        }

        public void KickPlayer(string code) {
            if (!TryGetLobby(code, out Lobby lobby)) {
                return;
            }
            if (!lobby.Players.TryGetValue("PlayerTwo", out Profile guest)) {
                return;
            }

            RemoveGuestFromLobby(guest, code, lobby);
            NotifyHostGuestLeft(lobby);

            if (lobby.Players.TryGetValue("PlayerOne", out Profile host)) {
                TryNotifyCallback(host.Username, callback => callback.GuestLeftCallback());
            }
        }

        private void RemoveGuestFromLobby(Profile guest, string lobbyCode, Lobby lobby) {
            TryNotifyCallback(guest.Username, callback => callback.KickedFromLobby());
            lobby.Players.Remove("PlayerTwo");
            _lobbyPlayerCallback.TryRemove(guest.Username, out _);
            LeaveChat(guest.Username, lobbyCode);
        }

        private void NotifyHostGuestLeft(Lobby lobby) {
            if (lobby.Players.TryGetValue("PlayerOne", out Profile host)) {
                TryNotifyCallback(host.Username, callback => callback.GuestLeftCallback());
            }
        }
        private string GetPlayer(Lobby lobby, string role) {
            string usernameRetrieved = Constants.NO_MATCHES_STRING;
            if (lobby.Players.TryGetValue(role, out Profile player)) {
                usernameRetrieved = player.Username;
            }
            return usernameRetrieved;
        }

        private void RemoveGuestFromLobby(Lobby lobby) {
            lobby.Players.Remove("PlayerTwo");
        }
    }
}