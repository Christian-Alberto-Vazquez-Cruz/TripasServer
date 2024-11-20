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

            // Obtiene o crea un nuevo diccionario para el lobby si no existe
            ConcurrentDictionary<string, IChatManagerCallBack> lobbyUsers = connectedUsersByLobby.GetOrAdd(lobbyCode,
                _ => new ConcurrentDictionary<string, IChatManagerCallBack>());

            // Intenta añadir el usuario al lobby
            if (lobbyUsers.TryAdd(userName, callback)) {
                Console.WriteLine($"{userName} se ha conectado al chat en el lobby {lobbyCode}");

                BroadcastMessageToLobby(new Message($"{userName} se ha unido al lobby.", DateTime.Now, userName), lobbyCode);
            } else {
                Console.WriteLine($"Usuario {userName} ya se encuentra conectado en el lobby {lobbyCode}.");
            }
        }

        // Método para que un usuario abandone el chat
        public void LeaveChat(string userName, string lobbyCode) {
            // Intenta obtener el diccionario de usuarios del lobby y eliminar al usuario
            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers) &&
                lobbyUsers.TryRemove(userName, out _)) {
                
                // Notifica a los demás usuarios que alguien ha abandonado el lobby
                BroadcastMessageToLobby(new Message($"User {userName} abandonó el lobby.", DateTime.Now, userName), lobbyCode);

                // Si el lobby queda vacío, lo elimina
                if (lobbyUsers.IsEmpty) {
                    connectedUsersByLobby.TryRemove(lobbyCode, out _);
                }
            }
        }

        // Método para enviar un mensaje en el chat
        public void SendMessage(string userName, Message message, string lobbyCode) {
            message.timeStamp = DateTime.Now;

            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers) &&
                lobbyUsers.ContainsKey(userName)) {
                BroadcastMessageToLobby(message, lobbyCode);
            } else {
                Console.WriteLine($"User {userName} no está conectado en el lobby {lobbyCode}.");
            }
        }

        //¿Mantengo el parámetro de userName o elimino userName de message?
        /*public void SendMessage(Message message, string lobbyCode) {
            message.timeStamp = DateTime.Now;

            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers)) {
                // Solo transmite el mensaje si el remitente está en el lobby
                if (lobbyUsers.ContainsKey(message.UserName)) {
                    BroadcastMessageToLobby(message, lobbyCode);
                } else {
                    Log($"User {message.UserName} no está conectado en el lobby {lobbyCode}.");
                }
            }*/

            // Método privado para difundir un mensaje a todos los usuarios de un lobby
            private void BroadcastMessageToLobby(Message message, string lobbyCode) {
            Console.WriteLine($"Broadcasting message to lobby {lobbyCode}: {message}");

            if (connectedUsersByLobby.TryGetValue(lobbyCode, out var lobbyUsers)) {
                foreach (var user in lobbyUsers.Values) {
                    try {
                        // Intenta enviar el mensaje a cada usuario
                        user.BroadcastMessage(message);
                    } catch (Exception exception) {
                        // Si hay un error, busca y elimina al usuario desconectado
                        string disconnectedUser = lobbyUsers.FirstOrDefault(x => x.Value == user).Key;
                        if (disconnectedUser != null) {
                            lobbyUsers.TryRemove(disconnectedUser, out _);
                            Console.WriteLine($"Excepción durante el broadcast para {disconnectedUser} en el lobby {lobbyCode}: {exception.Message}");
                        }   
                    }
                }
            }
        }
    }
}