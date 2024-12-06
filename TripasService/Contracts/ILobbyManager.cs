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

        /// <summary>
        /// Permits the leaving of a player using a unique lobby code.
        /// </summary>
        /// <param name="code">The unique identifier for the lobby.</param>
        /// <param name="username">The username of the player leaving the lobby.</param>
        [OperationContract(IsOneWay = true)]
        void LeaveLobby(string code, string username);

        /// <summary>
        /// Connects a player callback to a lobby using a unique lobby code.
        /// </summary>
        /// <param name="code">The unique identifier for the lobby.</param>
        /// <param name="playerId">The ID of the player being connected to the lobby.</param>
        /// <returns>Returns true if the player was successfully connected, false otherwise.</returns>
        [OperationContract]
        bool ConnectPlayerToLobby(string code, int playerId);

        /// <summary>
        /// Starts the match within the lobby. Notifies the players as well.
        /// </summary>
        /// <param name="code">The unique identifier for the lobby.</param>
        [OperationContract(IsOneWay = true)]
        void StartMatch(string code);

        /// <summary>
        /// Kicks a player from the lobby, typically used by the host.
        /// </summary>
        /// <param name="code">The unique identifier for the lobby.</param>
        [OperationContract(IsOneWay = true)]
        void KickPlayer(string code);

    }

    [ServiceContract]
    public interface ILobbyManagerCallback {

        /// <summary>
        /// Notifies a guest it has been kicked by the host.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void KickedFromLobby();

        /// <summary>
        /// Notifies a guest the host has left the lobby.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void HostLeftCallback();

        /// <summary>
        /// Notifies a host the guest has left the lobby.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void GuestLeftCallback();

        /// <summary>
        /// Notifies the host a guest has arrived.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void GuestJoinedCallback(string guestName, string picturePath, int idProfile);


        /// <summary>
        /// Notifies both players the game has started.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void GameStarted();
    }

}