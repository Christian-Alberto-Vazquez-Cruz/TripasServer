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
        public Dictionary<string, Profile> Players { get; set; } = new Dictionary<string, Profile> {
            { "PlayerOne", null },
            { "PlayerTwo", null }
        };

        [DataMember]
        public string Status { get; set; } 

        [DataMember]
        public int CurrentTurn { get; set; } 

        [DataMember]
        public List<Trace> Traces { get; set; } = new List<Trace>();

        [DataMember]
        public Dictionary<string, Node> Nodes { get; private set; } = new Dictionary<string, Node>();

        [DataMember]
        public Dictionary<string, string> NodePairs { get; private set; } = new Dictionary<string, string>();

        public void AddTrace(Trace trace) {
            Traces.Add(trace);
        }

        public string GetNodePair(string nodeId) {
            NodePairs.TryGetValue(nodeId, out var pairId);
            return pairId;
        }

        public List<Node> GetAllNodes() {
            return Nodes.Values.ToList();
        }

        public Dictionary<string, string> GetNodePairs() {
            return new Dictionary<string, string>(NodePairs);
        }


        public void StartGame() {
            GenerateNodes();
            PairNodes();
            Status = "InProgress";
            Console.WriteLine($"La partida {Code} ha comenzado con {NodeCount} nodos.");
        }

        public void EndGame() {
            Status = "Finished";
            Console.WriteLine($"La partida {Code} ha terminado.");
        }

        private void GenerateNodes() {
            var random = new Random();
            double canvasWidth = 848; // Ancho del área útil
            double canvasHeight = 555; // Alto del área útil

            for (int i = 0; i < NodeCount; i++) {
                var node = new Node {
                    Id = $"Node-{i}",
                    X = random.NextDouble() * canvasWidth,
                    Y = random.NextDouble() * canvasHeight
                };
                Nodes[node.Id] = node;
            }
        }

        /*private void PairNodes() {
            var nodeIds = Nodes.Keys.ToList();
            var random = new Random();
            nodeIds = nodeIds.OrderBy(_ => random.Next()).ToList(); // Mezclar nodos aleatoriamente

            for (int i = 0; i < nodeIds.Count - 1; i += 2) {
                NodePairs[nodeIds[i]] = nodeIds[i + 1];
                NodePairs[nodeIds[i + 1]] = nodeIds[i];
            }
        }*/

        private void PairNodes() {
            var nodeIds = Nodes.Keys.ToList();
            var random = new Random();
            nodeIds = nodeIds.OrderBy(_ => random.Next()).ToList(); // Mezclar nodos aleatoriamente

            var colors = new List<string> { "Red", "Blue", "Gray", "Green", "Purple", "Yellow", "Cyan" };
            int colorIndex = 0;

            for (int i = 0; i < nodeIds.Count - 1; i += 2) {
                NodePairs[nodeIds[i]] = nodeIds[i + 1];
                NodePairs[nodeIds[i + 1]] = nodeIds[i];

                // Asignar el color a ambos nodos de la pareja
                Nodes[nodeIds[i]].Color = colors[colorIndex];
                Nodes[nodeIds[i + 1]].Color = colors[colorIndex];

                // Avanzar al siguiente color
                colorIndex = (colorIndex + 1) % colors.Count;
            }
        }

        public Match(string code, string gameName, int nodeCount, Dictionary<string, Profile> players) {
            Code = code;
            GameName = gameName;
            NodeCount = nodeCount;
            Players = players;
            Status = "InProgress";
            CurrentTurn = 0; 
        }
    }
}
