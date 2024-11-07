using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Runtime.Serialization;
using System.ServiceModel;
using TripasService.Logic;

namespace TripasService.Contracts {

    
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager {


        [OperationContract(IsOneWay = true)] 
        void LeaveLobby(string code, int playerId);

    }

    [ServiceContract]
    public interface ILobbyManagerCallback {
        [OperationContract]
        void RemoveFromLobby();
        [OperationContract]
        void HostLeftCallback();
        [OperationContract]
        void GuestLeftCallback();
    }

}