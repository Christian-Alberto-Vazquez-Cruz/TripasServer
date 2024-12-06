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
        bool ConnectPlayerToLobby(string code, int playerId);

        [OperationContract(IsOneWay = true)]
        void StartMatch(string code);

        [OperationContract(IsOneWay = true)]
        void KickPlayer(string code);

    }

    [ServiceContract]
    public interface ILobbyManagerCallback {
        [OperationContract(IsOneWay = true)]
        void KickedFromLobby();
        [OperationContract(IsOneWay = true)]
        void HostLeftCallback();
        [OperationContract(IsOneWay = true)]
        void GuestLeftCallback();
        [OperationContract(IsOneWay = true)]
        void GuestJoinedCallback(string guestName, string picturePath, int idProfile);
        [OperationContract(IsOneWay = true)]
        void GameStarted();
    }

}