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

        public bool RegisterPlayerCallback(string matchCode, string username) {
            if (!activeMatches.ContainsKey(matchCode)) return false;

            var callback = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();
            return matchPlayerCallback.TryAdd(username, callback);
        }

        public bool RegisterTrace(string matchCode, Trace trace) {
            if (!activeMatches.ContainsKey(matchCode))
                return false;

            // Agregar el trazo a la partida
            activeMatches[matchCode].AddTrace(trace);

            // Notificar a los otros jugadores del trazo
            foreach (string playerName in activeMatches[matchCode].Players.Keys) {
                if (playerName != trace.Player && matchPlayerCallback.ContainsKey(playerName)) {
                    matchPlayerCallback[playerName].TraceReceived(trace);
                }
            }
            return true;
        }


    }
}
