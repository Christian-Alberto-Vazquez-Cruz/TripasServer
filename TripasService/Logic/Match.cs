using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class Match {
        [DataMember]
        public string Code { get; set; } 

        [DataMember]
        public string GameName { get; set; }

        [DataMember]
        public int NodeCount { get; set; }

        [DataMember]
        public Dictionary<string, Profile> Players { get; set; } = new Dictionary<string, Profile>();

        [DataMember]
        public string Status { get; set; } // Estado de la partida: En progreso, Finalizado, Pausado, etc.

        [DataMember]
        public int CurrentTurn { get; set; } // Para indicar de quién es el turno

        [DataMember]
        public List<Trace> Traces { get; set; } = new List<Trace>();

        public void AddTrace(Trace trace) {
            Traces.Add(trace);
        }

        public Match(string code, string gameName, int nodeCount, Dictionary<string, Profile> players) {
            Code = code;
            GameName = gameName;
            NodeCount = nodeCount;
            Players = players;
            Status = "InProgress";
            CurrentTurn = 0; // o cualquier lógica inicial de turno
        }

        public void StartGame() {
            Status = "InProgress";
            Console.WriteLine($"La partida {Code} ha comenzado.");
        }

        public void EndGame() {
            Status = "Finished";
            Console.WriteLine($"La partida {Code} ha terminado.");
        }
    }
}
