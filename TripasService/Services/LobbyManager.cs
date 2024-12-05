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
        private static readonly ConcurrentDictionary<string, Lobby> _lobbies = new ConcurrentDictionary<string, Lobby>();
        private static readonly ConcurrentDictionary<string, ILobbyManagerCallback> _lobbyPlayerCallback = new ConcurrentDictionary<string, ILobbyManagerCallback>();

        private bool TryNotifyCallback(string username, Action<ILobbyManagerCallback> callbackAction) {
            if (_lobbyPlayerCallback.TryGetValue(username, out var callback)) {
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

                _lobbyPlayerCallback.TryRemove(username, out _);
                Console.WriteLine($"Callback removed for {username} due to communication error");
            }
            return false;
        }

        public void LeaveLobby(string code, int playerId) {
            if (_lobbies.TryGetValue(code, out var lobby)) {
                if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.IdProfile == playerId) {
                    // Eliminar el callback del host
                    _lobbyPlayerCallback.TryRemove(host.Username, out _);
                    OnHostDisconnect(code);
                } else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.IdProfile == playerId) {
                    lobby.Players.Remove("PlayerTwo");
                    // Eliminar el callback del guest
                    _lobbyPlayerCallback.TryRemove(guest.Username, out _);

                    // Notificar al host que Guest abandonó
                    if (host != null) {
                        TryNotifyCallback(host.Username, callback => callback.GuestLeftCallback());
                    }
                }
            }
        }

        private void OnHostDisconnect(string code) {
            if (_lobbies.TryGetValue(code, out var lobby)) {
                if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest != null) {
                    // Notificar al guest que Host abadonó
                    TryNotifyCallback(guest.Username, callback => callback.HostLeftCallback());
                    // Eliminar el callback del guest ya que el lobby se cerrará
                    _lobbyPlayerCallback.TryRemove(guest.Username, out _);
                }
                DeleteLobby(code);
            }
        }

        public bool ConnectPlayerToLobby(string code, int playerId) {
            var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();

            if (!_lobbies.TryGetValue(code, out var lobby)) {
                Console.WriteLine($"Lobby with code {code} not found.");
                return false;
            }

            if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.IdProfile == playerId) {
                if (_lobbyPlayerCallback.TryAdd(host.Username, callback)) {
                    Console.WriteLine($"Host {host.Username} callback registered successfully.");
                    return true;
                } else {
                    Console.WriteLine($"Failed to register callback for host {host.Username}.");
                }
            } else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.IdProfile == playerId) {
                if (_lobbyPlayerCallback.TryAdd(guest.Username, callback)) {
                    Console.WriteLine($"Guest {guest.Username} callback registered successfully.");
                    if (TryNotifyCallback(host.Username, callbk => callbk.GuestJoinedCallback(guest.Username, guest.PicturePath, guest.IdProfile))) {
                        return true;
                    } else {
                        Console.WriteLine($"Failed to notify host {host.Username} about guest {guest.Username}.");
                        _lobbyPlayerCallback.TryRemove(guest.Username, out _);
                    }
                } else {
                    Console.WriteLine($"Failed to register callback for guest {guest.Username}.");
                }
            }

            Console.WriteLine("Connection to lobby failed.");
            return false;
        }


        public void StartMatch(string code) {
            if (!_lobbies.TryGetValue(code, out var lobby)) {
                Console.WriteLine($"Lobby con código {code} no encontrado.");
                return;
            }

            //Aquí no debería ser un Profile tampoco
            if (!lobby.Players.TryGetValue("PlayerOne", out Profile host)) {
                Console.WriteLine($"El lobby {code} no tiene un anfitrión válido.");
                return;
            }

            //Aquí no debería ser un Profile tampoco
            if (!lobby.Players.TryGetValue("PlayerTwo", out Profile guest)) {
                Console.WriteLine($"El lobby {code} no tiene suficientes jugadores para iniciar la partida.");
                return;
            }

            var match = new Match(
                code,
                lobby.GameName,
                lobby.NodeCount,
                new Dictionary<string, Profile>
                {
        { "PlayerOne", host },
        { "PlayerTwo", guest }
                }
            );

            match.StartGame();

            // Registrar la partida en el sistema
            if (!_activeMatches.TryAdd(code, match)) {
                Console.WriteLine($"Unable to register match with {code} code. Verify duplicity.");
                return;
            }

            NotifyPlayersMatchStarted(host, guest);
            RemoveLobbyCallbacks(code);
            RemoveChatCallbacks(code);

        }

        private void NotifyPlayersMatchStarted(Profile host, Profile guest) {
            TryNotifyCallback(host.Username, cb => cb.GameStarted());
            TryNotifyCallback(guest.Username, cb => cb.GameStarted());
        }

        private void RemoveLobbyCallbacks(string code) {
            if (_lobbies.TryGetValue(code, out var lobby)) {
                foreach (Profile player in lobby.Players.Values) {
                    if (_lobbyPlayerCallback.TryRemove(player.Username, out _)) {
                        Console.WriteLine($"El callback de lobby para {player.Username} ha sido eliminado del lobby {code}.");
                    }
                }
                DeleteLobby(code);
            } else {
                Console.WriteLine($"No se encontró ningún lobby con el código {code}.");
            }
        }

        public bool DeleteLobby(string code) {
            bool operationResult = _lobbies.TryRemove(code, out _);
            return operationResult;
        }

        public void KickPlayer(string code) {
            if (!_lobbies.TryGetValue(code, out var lobby)) {
                Console.WriteLine($"Lobby con código {code} no encontrado.");
                return;
            }

            if (!lobby.Players.TryGetValue("PlayerOne", out var host)) {
                Console.WriteLine($"El anfitrión del lobby {code} no es válido o no existe.");
                return;
            }

            if (!lobby.Players.TryGetValue("PlayerTwo", out var guest)) {
                Console.WriteLine($"No hay invitado en el lobby {code} para ser expulsado.");
                return;
            }

            lobby.Players.Remove("PlayerTwo");

            if (_lobbyPlayerCallback.TryRemove(guest.Username, out var guestCallback)) {
                try {
                    guestCallback.KickedFromLobby();
                } catch (Exception ex) {
                    Console.WriteLine($"Error al notificar al invitado {guest.Username} que fue expulsado: {ex.Message}");
                }
            }

            if (_lobbyPlayerCallback.TryGetValue(host.Username, out var hostCallback)) {
                try {
                    hostCallback.GuestLeftCallback();
                } catch (Exception ex) {
                    Console.WriteLine($"Error al notificar al anfitrión {host.Username} sobre la salida del invitado: {ex.Message}");
                }
            }

            Console.WriteLine($"El invitado {guest.Username} ha sido expulsado del lobby {code}.");

        }

    }
}