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
        void LeaveLobby(string code, string username);

        [OperationContract]
        bool ConnectPlayerToLobby(string code, string username);

        [OperationContract(IsOneWay = true)]
        void StartMatch(string code);


    }

    [ServiceContract]
    public interface ILobbyManagerCallback {
        [OperationContract]
        void RemoveFromLobby();
        [OperationContract]
        void HostLeftCallback();
        [OperationContract]
        void GuestLeftCallback();
        [OperationContract]
        void GuestJoinedCallback(string guestName);
        [OperationContract]
        void GameStarted();
    }

}