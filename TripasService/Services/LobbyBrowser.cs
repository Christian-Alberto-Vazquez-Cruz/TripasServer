using System.Linq;
using TripasService.Logic;
using TripasService.Utils;
using TripasService.Contracts;
using System.Collections.Generic;

namespace TripasService.Services {

    public partial class TripasGameService : ILobbyBrowser {

        public List<Lobby> GetAvailableLobbies() {
            return _lobbies.Values.ToList();
        }

        public bool JoinLobby(string code, Profile guest) {
            bool result = false;
            if (_lobbies.TryGetValue(code, out Lobby lobby) && lobby.HasSpace) {
                lobby.Players["PlayerTwo"] = guest;
                result = true;
            }
            return result;
        }

        public string CreateLobby(string gameName, int nodeCount, Profile owner) {
            string code = GenerateUniqueLobbyCode();

            Lobby newLobby = new Lobby(code, gameName, nodeCount, owner);
            if (_lobbies.TryAdd(code, newLobby)) {
                return code;
            }
            return Constants.FAILED_OPERATION_STRING;
        }

        private string GenerateUniqueLobbyCode() {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateLobbyCode();
            } while (_lobbies.ContainsKey(code));
            return code; 
        }

        public Lobby GetLobbyByCode(string code) {
            Lobby lobbyRetrieved = new Lobby {
                Code = Constants.FAILED_OPERATION_STRING
            };
            if (_lobbies.TryGetValue(code, out Lobby lobby)) {
                lobbyRetrieved = lobby;
            }
            return lobbyRetrieved;
        }
    }
}