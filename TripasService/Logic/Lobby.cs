using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;

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
        public TimeSpan Duration { get; set; }

        [DataMember]
        public Dictionary<string, Profile> Players { get; set; } = new Dictionary<string, Profile>();

        public Lobby(string code, string gameName, int nodeCount, Profile owner, TimeSpan duration) {
            Code = code;
            GameName = gameName;
            NodeCount = nodeCount;
            Players["PlayerOne"] = owner;
            Duration = duration;
        }
        public bool HasSpace => !Players.ContainsKey("PlayerTwo");
    }
}