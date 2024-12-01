using TripasService.Utils;
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

        private ConcurrentDictionary<string, ConcurrentDictionary<string, IChatManagerCallBack>> connectedUsersByLobby =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, IChatManagerCallBack>>();


        public void ConnectToChat(string userName, string lobbyCode) {
            var callback = OperationContext.Current.GetCallbackChannel<IChatManagerCallBack>();

            ConcurrentDictionary<string, IChatManagerCallBack> lobbyUsers = connectedUsersByLobby.GetOrAdd(lobbyCode,
                _ => new ConcurrentDictionary<string, IChatManagerCallBack>());

            if (lobbyUsers.TryAdd(userName, callback)) {
                BroadcastMessageToLobby(new Message($"{userName} se ha unido al lobby.", DateTime.Now, userName), lobbyCode);
            }
        }

        public void LeaveChat(string userName, string lobbyCode) {
            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers) &&
                lobbyUsers.TryRemove(userName, out _)) {

                BroadcastMessageToLobby(new Message($"User {userName} abandonó el lobby.", DateTime.Now, userName), lobbyCode);

                if (lobbyUsers.IsEmpty) {
                    connectedUsersByLobby.TryRemove(lobbyCode, out _);
                }
            }
        }

        public void SendMessage(string userName, Message message, string lobbyCode) {
            message.timeStamp = DateTime.Now;

            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers) &&
                lobbyUsers.ContainsKey(userName)) {
                BroadcastMessageToLobby(message, lobbyCode);
            } else {
                Console.WriteLine($"User {userName} no está conectado en el lobby {lobbyCode}.");
            }
        }


        private void BroadcastMessageToLobby(Message message, string lobbyCode) {
            Console.WriteLine($"Broadcasting message to lobby {lobbyCode}: {message}");

            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers)) {
                foreach (var user in lobbyUsers.Values) {
                    try {
                        user.BroadcastMessage(message);
                    } catch (Exception exception) {
                        string disconnectedUser = lobbyUsers.FirstOrDefault(lobby => lobby.Value == user).Key;
                        if (disconnectedUser != null) {
                            lobbyUsers.TryRemove(disconnectedUser, out _);
                            Console.WriteLine($"Excepción durante el broadcast para {disconnectedUser} en el lobby {lobbyCode}: {exception.Message}");
                        }
                    }
                }
            }
        }

        private void RemoveChatCallbacks(string code) {
            if (connectedUsersByLobby.TryGetValue(code, out var lobbyUsers)) {
                foreach (var username in lobbyUsers.Keys) {
                    lobbyUsers.TryRemove(username, out _);
                    Console.WriteLine($"El callback de chat para {username} ha sido eliminado del lobby {code}.");
                }

                DeleteLobbyChat(code);
            } else {
                Console.WriteLine($"No se encontró ningún lobby con el código {code}.");
            }
        }

        public bool DeleteLobbyChat(string code) {
            bool operationResult = connectedUsersByLobby.TryRemove(code, out _);
            return operationResult;
        }
    }
}