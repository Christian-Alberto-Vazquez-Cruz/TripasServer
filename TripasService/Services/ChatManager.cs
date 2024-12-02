using TripasService.Utils;
using TripasService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
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
            Console.WriteLine($"Broadcasting message to lobby {lobbyCode}: {message}");

            if (_connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers)) {
                foreach (var user in lobbyUsers.Values) {
                    try {
                        user.BroadcastMessage(message);
                    } catch (Exception exception) {
                        string disconnectedUser = lobbyUsers.FirstOrDefault(lobby => lobby.Value == user).Key;
                        if (disconnectedUser != null) {
                            //NUEVO
                            //LeaveChat(disconnectedUser, lobbyCode);

                            //ANTERIOR
                            lobbyUsers.TryRemove(disconnectedUser, out _);

                            Console.WriteLine($"Excepción durante el broadcast para {disconnectedUser} en el lobby {lobbyCode}: {exception.Message}");
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