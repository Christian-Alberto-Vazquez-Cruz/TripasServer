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

        private bool TryNotifyCallback(string username, Action<ILobbyManagerCallback> callbackAction) {
            if (lobbyPlayerCallback.TryGetValue(username, out var callback)) {
                try {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened) {
                        callbackAction(callback);
                        return true;
                    }
                } catch (CommunicationException ex) {
                    Console.WriteLine($"Communication error with {username}: {ex.Message}");
                } catch (TimeoutException ex) {
                    Console.WriteLine($"Timeout while notifying {username}: {ex.Message}");
                } catch (ObjectDisposedException ex) {
                    Console.WriteLine($"Channel was disposed for {username}: {ex.Message}");
                }

                lobbyPlayerCallback.TryRemove(username, out _);
                Console.WriteLine($"Callback removed for {username} due to communication error");
            }
            return false;
        }

        public void LeaveLobby(string code, string username) {
            if (lobbies.TryGetValue(code, out Lobby lobby)) {
                if (lobby.Players.TryGetValue("PlayerOne", out string hostUsername) && hostUsername == username) {
                    // Eliminar el callback del host
                    lobbyPlayerCallback.TryRemove(hostUsername, out _);
                    OnHostDisconnect(code);
                } else if (lobby.Players.TryGetValue("PlayerTwo", out string guestUsername) && guestUsername == username) {
                    lobby.Players.Remove("PlayerTwo");
                    // Eliminar el callback del guest
                    lobbyPlayerCallback.TryRemove(guestUsername, out _);
                    // Notificar al host que Guest abandonó
                    TryNotifyCallback(hostUsername, callback => callback.GuestLeftCallback());
                }
            }
        }

        private void OnHostDisconnect(string code) {
            if (lobbies.TryGetValue(code, out Lobby lobby)) {
                if (lobby.Players.TryGetValue("PlayerTwo", out string guestUsername)) {
                    // Notificar al guest que Host abadonó
                    TryNotifyCallback(guestUsername, callback => callback.HostLeftCallback());
                    // Eliminar el callback del guest ya que el lobby se cerrará
                    lobbyPlayerCallback.TryRemove(guestUsername, out _);
                }
                DeleteLobby(code);
            }
        }

        public bool ConnectPlayerToLobby(string code, string username) {
            var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();

            if (lobbies.TryGetValue(code, out Lobby lobby)) {
                if (lobby.Players.TryGetValue("PlayerOne", out var hostUsername) && hostUsername == username) {
                    if (lobbyPlayerCallback.TryAdd(hostUsername, callback)) {
                        Console.WriteLine($"Host {hostUsername} callback registered successfully");
                        return true;
                    }
                } else if (lobby.Players.TryGetValue("PlayerTwo", out string guestUsername) && guestUsername == username) {
                    if (lobbyPlayerCallback.TryAdd(guestUsername, callback)) {
                        Console.WriteLine($"Guest {guestUsername} callback registered successfully");

                        // Notificar al host que se unió un Guest
                        if (TryNotifyCallback(hostUsername, callbk => callbk.GuestJoinedCallback(guestUsername))) {
                            return true;
                        } else {
                            // Limpiar callback si ocurre una excepción en el Host  
                            lobbyPlayerCallback.TryRemove(guestUsername, out _);
                        }
                    }
                }
            }
            return false;   
        }
        public void StartMatch(string code) {
            if (!lobbies.TryGetValue(code, out var lobby)) {
                Console.WriteLine($"Lobby con código {code} no encontrado.");
                return;
            }

            if (!lobby.Players.TryGetValue("PlayerOne", out var hostUsername)) {
                Console.WriteLine($"El lobby {code} no tiene un anfitrión válido.");
                return;
            }

            if (!lobby.Players.TryGetValue("PlayerTwo", out var guestUsername)) {
                Console.WriteLine($"El lobby {code} no tiene suficientes jugadores para iniciar la partida.");
                return;
            }

            var match = new Match(
                code,
                lobby.GameName,
                lobby.NodeCount,
                new Dictionary<string, string>
                {
                    { "PlayerOne", hostUsername },
                    { "PlayerTwo", guestUsername }
                }
            );

            match.StartGame();

            // Registrar la partida en el sistema
            if (!activeMatches.TryAdd(code, match)) {
                Console.WriteLine($"Unable to register match with {code} code. Verify duplicity.");
                return;
            }

            DeleteLobby(code);
            NotifyPlayersMatchStarted(hostUsername, guestUsername);
        }

        private void NotifyPlayersMatchStarted(string hostUsername, string guestUsername) {
            TryNotifyCallback(hostUsername, cb => cb.GameStarted());
            TryNotifyCallback(guestUsername, cb => cb.GameStarted());
        }


        public bool DeleteLobby(string code) {
            return lobbies.TryRemove(code, out _);
        }
    }
}