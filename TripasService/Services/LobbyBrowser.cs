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
            if (_lobbies.TryGetValue(code, out var lobby) && lobby.HasSpace) {
                lobby.Players["PlayerTwo"] = guest;
                result = true;
            }
            return result;
        }

        //AQUÍ SE DEBE INICIALIZAR UNA CADENA VACÍA. ¿CÓMO SE HACE?
        //TAMBIÉN QUITAR EL TIMESPAN
        public string CreateLobby(string gameName, int nodeCount, Profile owner) {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateLobbyCode();
            } while (_lobbies.ContainsKey(code));

            var newLobby = new Lobby(code, gameName, nodeCount, owner);
            if (_lobbies.TryAdd(code, newLobby)) {
                return code;
            }
            return null;
        }

        public Lobby GetLobbyByCode(string code) {
            if (!_lobbies.TryGetValue(code, out Lobby lobby)) {
                throw new KeyNotFoundException($"Lobby with code '{code}' not found.");
            }
            return lobby;
        }
    }
}