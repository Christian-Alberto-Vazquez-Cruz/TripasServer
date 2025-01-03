﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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
        public Dictionary<string, Node> Nodes { get; private set; } = new Dictionary<string, Node>();

        [DataMember]
        public Dictionary<string, string> NodePairs { get; private set; } = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, int> CurrentScores { get; private set; } = new Dictionary<string, int>();

        [DataMember]
        public string CurrentTurn { get; set; }

        public void SwitchTurn() {
            if (Players["PlayerOne"] == null || Players["PlayerTwo"] == null) return;

            CurrentTurn = CurrentTurn == Players["PlayerOne"].Username
                ? Players["PlayerTwo"].Username
                : Players["PlayerOne"].Username;
        }

        private static readonly List<(double X, double Y)> ValidCoordinates = new List<(double, double)> {
            (175, 48),(235, 50),(140, 60),(100, 80),(175, 80),(270, 90),(355, 105),(130, 115),(52, 145),
            (150, 143),(200, 145),(250, 130),(280, 130),(325, 125),(380, 130),(440, 125),(90, 155),(55, 180),(140, 180),
            (155, 175),(215, 180),(270, 155),(300, 175),(350, 180),(380, 160),(405, 155),(445, 175),(100, 200),(60, 225),
            (140, 225),(180, 230),(250, 250),(270, 215),(320, 210),(370, 220),(410, 205),(460, 200),(450, 245),(510, 245),
            (40, 260),(100, 260),(175, 275),(200, 290),(325, 275),(480, 250),(360, 300),(330, 310),(60, 295),(380, 300),
        };

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
            CurrentTurn = Players["PlayerOne"].Username;
            GenerateNodes();
            PairNodes();
            InitializeScores();
        }

        public void InitializeScores() {
            foreach (Profile player in Players.Values) {
                if (player != null) {
                    CurrentScores[player.Username] = 0;
                }
            }
        }
        private void GenerateNodes() {
            Random random = new Random();

            var selectedCoordinates = ValidCoordinates
                .OrderBy(_ => random.Next())
                .Take(NodeCount)
                .ToList();

            for (int i = 0; i < selectedCoordinates.Count; i++) {
                var (x, y) = selectedCoordinates[i];
                Node node = new Node {
                    Id = $"Node-{i}",
                    X = x,
                    Y = y
                };
                Nodes[node.Id] = node;
            }
        }

        private void PairNodes() {
            List<string> nodeIds = Nodes.Keys.ToList();
            Random random = new Random();
            nodeIds = nodeIds.OrderBy(_ => random.Next()).ToList();

            List<string> colors = new List<string> { "Red", "Blue", "Gray", "Green", "Purple","Yellow", "White", "Orange", "Magenta", "Brown" };
            int colorIndex = 0;

            for (int i = 0; i < nodeIds.Count - 1; i += 2) {
                NodePairs[nodeIds[i]] = nodeIds[i + 1];
                NodePairs[nodeIds[i + 1]] = nodeIds[i];

                Nodes[nodeIds[i]].Color = colors[colorIndex];
                Nodes[nodeIds[i + 1]].Color = colors[colorIndex];

                colorIndex = (colorIndex + 1) % colors.Count;
            }
        }

        public void AddPoints(string player, int points) {
            if (CurrentScores.ContainsKey(player)) {
                CurrentScores[player] += points;
            }
        }

        public int GetPlayerScore(string player) {
            return CurrentScores.TryGetValue(player, out int score) ? score : 0;
        }

        public Match(string code, string gameName, int nodeCount, Dictionary<string, Profile> players) {
            Code = code;
            GameName = gameName;
            NodeCount = nodeCount;
            Players = players;
        }

        public Match() {
        }
    }
}