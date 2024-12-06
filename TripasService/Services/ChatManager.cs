using System;
using System.Linq;
using System.ServiceModel;
using TripasService.Logic;
using TripasService.Contracts;
using System.Collections.Concurrent;

namespace TripasService.Services {

    public partial class TripasGameService : IChatManager {

        private ConcurrentDictionary<string, ConcurrentDictionary<string, IChatManagerCallBack>> _connectedUsersByLobby =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, IChatManagerCallBack>>();

        public void ConnectToChat(string username, string lobbyCode) {
            var callback = OperationContext.Current.GetCallbackChannel<IChatManagerCallBack>();
            ConcurrentDictionary<string, IChatManagerCallBack> lobbyUsers = _connectedUsersByLobby.GetOrAdd(lobbyCode,
                _ => new ConcurrentDictionary<string, IChatManagerCallBack>());
            if (lobbyUsers.TryAdd(username, callback)) {
                BroadcastMessageToLobby(new Message($"{username} se ha unido al lobby.", username), lobbyCode);
            }
        }

        public void SendMessage(string username, Message message, string lobbyCode) {
            if (_connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers) &&
                lobbyUsers.ContainsKey(username)) {
                BroadcastMessageToLobby(message, lobbyCode);
            }
        }

        private void BroadcastMessageToLobby(Message message, string lobbyCode) {
            LoggerManager logger = new LoggerManager(this.GetType());
            if (_connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers)) {
                foreach (var user in lobbyUsers.Values) {
                    try {
                        user.BroadcastMessage(message);
                    } catch (CommunicationException communicationException) {
                        logger.LogError($"Error while broadcasting message to user in lobby {lobbyCode}: {communicationException.Message}", communicationException);
                        string disconnectedUser = lobbyUsers.FirstOrDefault(lobby => lobby.Value == user).Key;
                        if (disconnectedUser != null) {
                            LeaveChat(disconnectedUser, lobbyCode);
                            LeaveLobby(lobbyCode, disconnectedUser);
                        }
                    } catch (TimeoutException timeoutException) {
                        logger.LogError($"Error while broadcasting message to user in lobby {lobbyCode}: {timeoutException.Message}", timeoutException);
                        string disconnectedUser = lobbyUsers.FirstOrDefault(lobby => lobby.Value == user).Key;
                        if (disconnectedUser != null) {
                            LeaveChat(disconnectedUser, lobbyCode);
                            LeaveLobby(lobbyCode, disconnectedUser);
                        }
                    } catch (Exception exception) {
                        logger.LogError($"Error while broadcasting message to user in lobby {lobbyCode}: {exception.Message}", exception);
                        string disconnectedUser = lobbyUsers.FirstOrDefault(lobby => lobby.Value == user).Key;
                        if (disconnectedUser != null) {
                            LeaveChat(disconnectedUser, lobbyCode);
                            LeaveLobby(lobbyCode, disconnectedUser);
                        }
                    }
                }
            }
        }

        public void LeaveChat(string username, string lobbyCode) {
            if (_connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers) &&
                lobbyUsers.TryRemove(username, out _)) {
                BroadcastMessageToLobby(new Message($"User {username} abandonó el lobby.", username), lobbyCode);
                if (lobbyUsers.IsEmpty) {
                    _connectedUsersByLobby.TryRemove(lobbyCode, out _);
                }
            }
        }

        private void RemoveChatCallbacks(string code) {
            if (_connectedUsersByLobby.TryGetValue(code, out var lobbyUsers)) {
                foreach (var username in lobbyUsers.Keys) {
                    lobbyUsers.TryRemove(username, out _);
                }
                DeleteLobbyChat(code);
            }

        }

        public bool DeleteLobbyChat(string code) {
            bool operationResult = _connectedUsersByLobby.TryRemove(code, out _);
            return operationResult;
        }
    }
}