using System;
using TripasService.Utils;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Services;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IStatusManager {

        /// <summary>
        /// Sets the status of a player.
        /// </summary>
        /// <param name="idProfile">The ID of the player whose status will be updated.</param>
        /// <param name="status">The new status to assign to the player.</param>
        /// <returns>Returns true if the operation was successful.</returns>
        [OperationContract]
        bool SetPlayerStatus(int idProfile, GameEnums.PlayerStatus status);

        /// <summary>
        /// Retrieves the current status of a player.
        /// </summary>
        /// <param name="idProfile">The ID of the player whose status will be retrieved.</param>
        /// <returns>The current status of the player.</returns>
        [OperationContract]
        GameEnums.PlayerStatus GetPlayerStatus(int idProfile);

        /// <summary>
        /// Disconnects a player by removing their status from the system.
        /// </summary>
        /// <param name="idProfile">The ID of the player to disconnect.</param>
        /// <returns>Returns 1 if the player was successfully disconnected, otherwise -2.</returns>
        [OperationContract]
        int DisconnectPlayer(int idProfile);

    } 
}
