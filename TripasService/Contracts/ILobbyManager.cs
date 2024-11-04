using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface ILobbyManager {
        [OperationContract]
        string CreateLobby(string gameName, int nodeCount, Profile owner);

        [OperationContract]
        Lobby GetLobbyByCode(string code);

        [OperationContract]
        List<Lobby> GetAvailableLobbies();

        [OperationContract]
        bool JoinLobby(string code, Profile guest);

        [OperationContract]
        bool LeaveLobby(string code, int playerId);

        [OperationContract]
        bool DeleteLobby(string code);
    }

    [DataContract]
    public class Lobby {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string GameName { get; set; }
        [DataMember]
        public int NodeCount { get; set; }
        [DataMember]
        public Dictionary<string, Profile> Players { get; set; } = new Dictionary<string, Profile>();

        public Lobby(string code, string gameName, int nodeCount, Profile owner) {
            Code = code;
            GameName = gameName;
            NodeCount = nodeCount;
            Players["PlayerOne"] = owner;
        }

        public bool HasSpace => !Players.ContainsKey("PlayerTwo");

    }

}