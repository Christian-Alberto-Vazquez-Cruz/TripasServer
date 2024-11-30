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

    public void LeaveLobby(string code, int playerId) {
        if (lobbies.TryGetValue(code, out var lobby)) {
            if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.IdProfile == playerId) {
                // Eliminar el callback del host
                lobbyPlayerCallback.TryRemove(host.Username, out _);
                OnHostDisconnect(code);
            } else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.IdProfile == playerId) {
                lobby.Players.Remove("PlayerTwo");
                // Eliminar el callback del guest
                lobbyPlayerCallback.TryRemove(guest.Username, out _);

                // Notificar al host que Guest abandonó
                if (host != null) {
                    TryNotifyCallback(host.Username, callback => callback.GuestLeftCallback());
                }
            }
        }
    }

    private void OnHostDisconnect(string code) {
        if (lobbies.TryGetValue(code, out var lobby)) {
            if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest != null) {
                // Notificar al guest que Host abadonó
                TryNotifyCallback(guest.Username, callback => callback.HostLeftCallback());
                // Eliminar el callback del guest ya que el lobby se cerrará
                lobbyPlayerCallback.TryRemove(guest.Username, out _);
            }
            DeleteLobby(code);
        }
    }

    public bool ConnectPlayerToLobby(string code, int playerId) {
        var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();

        if (lobbies.TryGetValue(code, out var lobby)) {
            if (lobby.Players.TryGetValue("PlayerOne", out var host) && host.IdProfile == playerId) {
                if (lobbyPlayerCallback.TryAdd(host.Username, callback)) {
                    Console.WriteLine($"Host {host.Username} callback registered successfully");
                    return true;
                }
            } else if (lobby.Players.TryGetValue("PlayerTwo", out var guest) && guest.IdProfile == playerId) {
                if (lobbyPlayerCallback.TryAdd(guest.Username, callback)) {
                    Console.WriteLine($"Guest {guest.Username} callback registered successfully");

                    // Notificar al host que se unió un Guest
                    if (TryNotifyCallback(host.Username, callbk => callbk.GuestJoinedCallback(guest.Username, guest.PicturePath))) {
                        return true;
                    } else {
                        // Limpiar callback si ocurre una excepción en el Host
                        lobbyPlayerCallback.TryRemove(guest.Username, out _);
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
        if (!activeMatches.TryAdd(code, match)) {
            Console.WriteLine($"Unable to register match with {code} code. Verify duplicity.");
            return;
        }

        lobbies.TryRemove(code, out _);
        NotifyPlayersMatchStarted(host, guest);


    }

    private void NotifyPlayersMatchStarted(Profile host, Profile guest) {
        TryNotifyCallback(host.Username, cb => cb.GameStarted());
        TryNotifyCallback(guest.Username, cb => cb.GameStarted());
    }


    public bool DeleteLobby(string code) {
        return lobbies.TryRemove(code, out _);
    }
}
}