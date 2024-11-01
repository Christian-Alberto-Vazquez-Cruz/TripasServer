using System;
using System.Collections.Concurrent;
using TripasService.Utils;
using TripasService.Contracts;


namespace TripasService.Services {
    public partial class TripasGameService : IStatusManager {
        private static ConcurrentDictionary<int, GameEnums.PlayerStatus> playerStatuses = new ConcurrentDictionary<int, GameEnums.PlayerStatus>();

        public GameEnums.PlayerStatus GetPlayerStatus(int idProfile) {
            if (playerStatuses.TryGetValue(idProfile, out GameEnums.PlayerStatus status)) {
                return status;
            }
            return GameEnums.PlayerStatus.Offline;
        }

        public bool SetPlayerStatus(int idProfile, GameEnums.PlayerStatus status) {
            playerStatuses[idProfile] = status;
            return true;
        }

        int IStatusManager.DisconnectPlayer(int idProfile) {
            int result = Constants.NO_MATCHES;
            if (playerStatuses.TryRemove(idProfile, out _)) {
                result = Constants.SUCCESSFUL_OPERATION;
            }
            return result;
        }
    }
}