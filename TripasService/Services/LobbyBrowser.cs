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
            if (lobbies.TryGetValue(code, out var lobby) && lobby.HasSpace) {
                lobby.Players["PlayerTwo"] = guest;
                return true;
            }
            return false;
        }
        public string CreateLobby(string gameName, int nodeCount, Profile owner) {
            string code;
            do {
                code = CodesGeneratorHelper.GenerateLobbyCode();
            } while (lobbies.ContainsKey(code));

            var newLobby = new Lobby(code, gameName, nodeCount, owner);
            if (lobbies.TryAdd(code, newLobby)) {
                //ESTO HACÍA ANTES
                //ILobbyManagerCallback callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
                //lobbyPlayerCallback[owner.userName] = callback;
                return code;
            }
            return null;
        }
        public Lobby GetLobbyByCode(string code) {
            lobbies.TryGetValue(code, out Lobby lobby);
            return lobby;
        }
    }
}

    
