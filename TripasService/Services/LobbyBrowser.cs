using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using TripasService.Logic;
using TripasService.Utils;

namespace TripasService.Services {
    public partial class TripasGameService : ILobbyBrowser {
        public List<Lobby> GetAvailableLobbies() {
            return lobbies.Values.ToList();
        }

        public bool JoinLobby(string code, Profile guest) {
            bool result = false;
            if (lobbies.TryGetValue(code, out var lobby) && lobby.HasSpace) {
                lobby.Players["PlayerTwo"] = guest;
                result = true;
            }
            return result;
        }

        //AQUÍ SE DEBE INICIALIZAR UNA CADENA VACÍA. ¿CÓMO SE HACE?
        //TAMBIÉN QUITAR EL TIMESPAN
        public string CreateLobby(string gameName, int nodeCount, Profile owner, TimeSpan duration) {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateLobbyCode();
            } while (lobbies.ContainsKey(code));

            var newLobby = new Lobby(code, gameName, nodeCount, owner, duration);
            if (lobbies.TryAdd(code, newLobby)) {
                return code;
            }
            return null;
        }
        public Lobby GetLobbyByCode(string code) {
            if (!lobbies.TryGetValue(code, out Lobby lobby)) {
                throw new KeyNotFoundException($"Lobby with code '{code}' not found.");
            }
            return lobby;
        }
    }
}