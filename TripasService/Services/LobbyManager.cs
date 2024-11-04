using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TripasService.Contracts;
using TripasService.Utils;

namespace TripasService.Services {
    public partial class TripasGameService : ILobbyManager {
        private static ConcurrentDictionary<string, Lobby> lobbies = new ConcurrentDictionary<string, Lobby>();

        public string CreateLobby(string gameName, int nodeCount, Profile owner) {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateLobbyCode();
            } while (lobbies.ContainsKey(code));

            var newLobby = new Lobby(code, gameName, nodeCount, owner);
            if (lobbies.TryAdd(code, newLobby)) {
                return code;
            }
            return null;
        }

        public bool DeleteLobby(string code) {
            return lobbies.TryRemove(code, out _);
        }

        public List<Lobby> GetAvailableLobbies() {
            return lobbies.Values.ToList();
        }

        public Lobby GetLobbyByCode(string code) {
            lobbies.TryGetValue(code, out var lobby);
            return lobby;
        }

        public bool JoinLobby(string code, Profile guest) {
            if (lobbies.TryGetValue(code, out var lobby) && lobby.HasSpace) {
                lobby.Players["PlayerTwo"] = guest;
                return true;
            }
            return false;
        }

        public bool LeaveLobby(string code, int playerId) {
            if (lobbies.TryGetValue(code, out var lobby)) {
                if (lobby.Players.ContainsKey("PlayerOne") && lobby.Players["PlayerOne"].idProfile == playerId) {
                    DeleteLobby(code);
                    return true;
                } else if (lobby.Players.ContainsKey("PlayerTwo") && lobby.Players["PlayerTwo"].idProfile == playerId) {
                    lobby.Players.Remove("PlayerTwo");
                    return true;
                }
            }
            return false;
        }
    }
}