using System;
using System.Collections.Concurrent;
using TripasService.Utils;
using TripasService.Contracts;


namespace TripasService.Services {
    public partial class TripasGameService : IStatusManager {
        private static readonly ConcurrentDictionary<int, GameEnums.PlayerStatus> _playerStatuses = new ConcurrentDictionary<int, GameEnums.PlayerStatus>();

        public GameEnums.PlayerStatus GetPlayerStatus(int idProfile) {
            if (_playerStatuses.TryGetValue(idProfile, out GameEnums.PlayerStatus status)) {
                return status;
            }
            return GameEnums.PlayerStatus.Offline;
        }

        public bool SetPlayerStatus(int idProfile, GameEnums.PlayerStatus status) {
            _playerStatuses[idProfile] = status;
            return true;
        }

        int IStatusManager.DisconnectPlayer(int idProfile) {
            int result = Constants.NO_MATCHES;
            if (_playerStatuses.TryRemove(idProfile, out _)) {
                result = Constants.SUCCESSFUL_OPERATION;
            }
            return result;
        }
    }
}