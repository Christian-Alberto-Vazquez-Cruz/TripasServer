using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using TripasService.Logic;

namespace TripasService.Services {
    public partial class TripasGameService : IMatchManager {

        private static ConcurrentDictionary<string, Match> activeMatches = new ConcurrentDictionary<string, Match>();
        private static ConcurrentDictionary<string, IMatchManagerCallback> matchPlayerCallback = new ConcurrentDictionary<string, IMatchManagerCallback>();
        private readonly Dictionary<string, Match> matches = new Dictionary<string, Match>();


        public List<Node> GetNodes(string matchCode) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return null;
            return match.GetAllNodes();
        }

        // NUEVO: Obtener las parejas de nodos de un juego
        public Dictionary<string, string> GetNodePairs(string matchCode) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return null;
            return match.GetNodePairs();
        }

        public bool RegisterPlayerCallback(string matchCode, string username) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return false;

            var callback = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            // Verificar si el usuario pertenece a la partida
            if (match.Players.Values.Any(player => player.userName == username)) {
                return matchPlayerCallback.TryAdd(username, callback);
            }

            Console.WriteLine($"El jugador {username} no pertenece a la partida {matchCode}.");
            return false;
        }
        public bool RegisterTrace(string matchCode, Trace trace) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return false;

            if (!match.IsPlayerTurn(trace.Player)) {
                Console.WriteLine($"El jugador {trace.Player} intentó dibujar fuera de su turno.");
                return false; // El jugador no está en turno.
            }

            match.AddTrace(trace);

            foreach (var player in match.Players.Values) {
                if (player.userName != trace.Player && matchPlayerCallback.TryGetValue(player.userName, out var callback)) {
                    try {
                        callback.TraceReceived(trace);
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al notificar al jugador {player.userName}: {ex.Message}");
                        matchPlayerCallback.TryRemove(player.userName, out _);
                    }
                }
            }

            // Cambiar el turno al siguiente jugador.
            SwitchTurn(matchCode);
            return true;
        }

        public bool IsPlayerTurn(string matchCode, string playerName) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return false;
            return match.IsPlayerTurn(playerName);
        }
        public async void SwitchTurn(string matchCode) {
            if (!activeMatches.TryGetValue(matchCode, out var match)) return;

            match.SwitchTurn();

            foreach (var player in match.Players.Values) {
                if (matchPlayerCallback.TryGetValue(player.userName, out var callback)) {
                    try {
                        await Task.Run(() => callback.TurnChanged(match.CurrentPlayerTurn));
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al notificar turno a {player.userName}: {ex.Message}");
                    }
                }
            }
        }

    }
}
