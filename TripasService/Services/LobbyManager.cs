﻿using System;
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

        private bool TryNotifyCallback(string userName, Action<ILobbyManagerCallback> callbackAction) {
            if (lobbyPlayerCallback.TryGetValue(userName, out var callback)) {
                try {
                    // Verificar si el canal está vivox|
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
                        callbackAction(callback);
                        return true;
                    }
                } catch (CommunicationException ex) {
                    Console.WriteLine($"Communication error with {userName}: {ex.Message}");
                } catch (TimeoutException ex) {
                    Console.WriteLine($"Timeout while notifying {userName}: {ex.Message}");
                } catch (ObjectDisposedException ex) {
                    Console.WriteLine($"Channel was disposed for {userName}: {ex.Message}");
                }

                // Si llegamos aquí, hubo un error y debemos limpiar el callback
                lobbyPlayerCallback.TryRemove(userName, out _);
                Console.WriteLine($"Callback removed for {userName} due to communication error");
            }
            return false;
        }

        public void LeaveLobby(string code, int playerId) {
            if (lobbies.TryGetValue(code, out var lobby)) {
                if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.idProfile == playerId) {
                    // Eliminar el callback del host
                    lobbyPlayerCallback.TryRemove(host.userName, out _);
                    OnHostDisconnect(code);
                } else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.idProfile == playerId) {
                    lobby.Players.Remove("PlayerTwo");
                    // Eliminar el callback del guest
                    lobbyPlayerCallback.TryRemove(guest.userName, out _);

                    // Notificar al host usando el método seguro
                    if (host != null) {
                        TryNotifyCallback(host.userName, callback => callback.GuestLeftCallback());
                    }
                }
            }
        }

        private void OnHostDisconnect(string code) {
            if (lobbies.TryGetValue(code, out var lobby)) {
                if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest != null) {
                    // Notificar al guest usando el método seguro
                    TryNotifyCallback(guest.userName, callback => callback.HostLeftCallback());
                    // Eliminar el callback del guest ya que el lobby se cerrará
                    lobbyPlayerCallback.TryRemove(guest.userName, out _);
                }
                DeleteLobby(code);
            }
        }

        public bool ConnectPlayerToLobby(string code, int playerId) {
            var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();

            if (lobbies.TryGetValue(code, out var lobby)) {
                if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.idProfile == playerId) {
                    if (lobbyPlayerCallback.TryAdd(host.userName, callback)) {
                        Console.WriteLine($"Host {host.userName} callback registered successfully");
                        return true;
                    }
                } else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.idProfile == playerId) {
                    if (lobbyPlayerCallback.TryAdd(guest.userName, callback)) {
                        Console.WriteLine($"Guest {guest.userName} callback registered successfully");

                        // Notificar al host usando el método seguro
                        if (TryNotifyCallback(host.userName, callbk => callbk.GuestJoinedCallback(guest.userName))) {
                            return true;
                        } else {
                            // Si no se pudo notificar al host, limpiamos el callback del guest
                            lobbyPlayerCallback.TryRemove(guest.userName, out _);
                        }
                    }
                }
            }
            return false;   
        }

        public bool DeleteLobby(string code) {
            return lobbies.TryRemove(code, out _);
        }
    }
}