using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TripasService.Logic;
using TripasService.Contracts;
using TripasService.Utils;
using System.ServiceModel;

namespace TripasService.Services {
    public partial class TripasGameService : ILobbyManager {
        private static ConcurrentDictionary<string, Lobby> lobbies = new ConcurrentDictionary<string, Lobby>();
        private static ConcurrentDictionary<string, ILobbyManagerCallback> lobbyPlayerCallback = new ConcurrentDictionary<string, ILobbyManagerCallback>();
        public void LeaveLobby(string code, int playerId) {
            if (lobbies.TryGetValue(code, out var lobby)) {
                // Verifica si el jugador que sale es el host (PlayerOne)
                if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.idProfile == playerId) {
                    OnHostDisconnect(code);  // Elimina el lobby y notifica al invitado si existe
                }
                // Si el jugador es el invitado (PlayerTwo)
                else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.idProfile == playerId) {
                    lobby.Players.Remove("PlayerTwo");  // Elimina al invitado del lobby

                    // Notifica al host que el invitado ha abandonado la sala
                    if (lobbyPlayerCallback.TryGetValue(host.userName, out var hostCallback)) {
                        hostCallback.GuestLeftCallback();
                    }
                }
            }
        }

        private void OnHostDisconnect(string code) {
            if (lobbies.TryGetValue(code, out var lobby)) {
                // Verifica si "PlayerTwo" está conectado y, si es así, envía el callback
                if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest != null) {
                    if (lobbyPlayerCallback.TryGetValue(guest.userName, out var guestCallback)) {
                        guestCallback.HostLeftCallback();  // Notifica al invitado que el host se fue
                    }
                }
                // Elimina el lobby
                DeleteLobby(code);
            }
        }

        public bool DeleteLobby(string code) {
            return lobbies.TryRemove(code, out _);
        }

    }
}