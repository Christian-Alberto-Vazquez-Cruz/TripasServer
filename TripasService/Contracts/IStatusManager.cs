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
        [OperationContract]
        bool SetPlayerStatus(int idProfile, GameEnums.PlayerStatus status);

        [OperationContract]
        GameEnums.PlayerStatus GetPlayerStatus(int idProfile);

        [OperationContract]
        int DisconnectPlayer(int idProfile);

    } 
}
