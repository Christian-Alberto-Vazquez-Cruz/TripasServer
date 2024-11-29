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

        public bool JoinLobby(string code,string guestUsername) {
            bool result = false;
            if (lobbies.TryGetValue(code, out Lobby lobby) && lobby.HasSpace) {
                lobby.Players["PlayerTwo"] = guestUsername;
                result = true;
            }
            return result;
        }

        //AQUÍ SE DEBE INICIALIZAR UNA CADENA VACÍA. ¿CÓMO SE HACE?

        //TAMBIÉN QUITAR EL TIMESPAN
        public string CreateLobby(string gameName, int nodeCount, string ownerUsername, TimeSpan duration) {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateLobbyCode();
            } while (lobbies.ContainsKey(code));

            Lobby newLobby = new Lobby(code, gameName, nodeCount, ownerUsername, duration);
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

    
