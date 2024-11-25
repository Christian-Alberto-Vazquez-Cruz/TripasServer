﻿using TripasService.Utils;
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

        //Obtiene el diccionario o crea uno nuevo
        public void ConnectToChat(string userName, string lobbyCode) {
            var callback = OperationContext.Current.GetCallbackChannel<IChatManagerCallBack>();

            ConcurrentDictionary<string, IChatManagerCallBack> lobbyUsers = connectedUsersByLobby.GetOrAdd(lobbyCode,
                _ => new ConcurrentDictionary<string, IChatManagerCallBack>());

            if (lobbyUsers.TryAdd(userName, callback)) {
                Console.WriteLine($"{userName} se ha conectado al chat en el lobby {lobbyCode}");

                BroadcastMessageToLobby(new Message($"{userName} se ha unido al lobby.", DateTime.Now, userName), lobbyCode);
            } else {
                Console.WriteLine($"Usuario {userName} ya se encuentra conectado en el lobby {lobbyCode}.");
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

        // Si hay un error, busca y elimina al usuario desconectado
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
    }
}