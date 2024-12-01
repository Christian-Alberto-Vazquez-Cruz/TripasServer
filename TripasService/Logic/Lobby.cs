using TripasService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using DataBaseManager.DAO;

namespace TripasService.Logic {
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

        public Lobby(string code, string gameName, int nodeCount, Profile host) {
            Code = code;
            GameName = gameName;
            NodeCount = nodeCount;
            Players["PlayerOne"] = host;
        }

        //CAMBIAR
        public bool HasSpace => !Players.ContainsKey("PlayerTwo");

    }
}
