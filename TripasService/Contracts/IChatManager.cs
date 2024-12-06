using System;
using System.Collections.Generic;
using System.Linq;
using TripasService.Logic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract(CallbackContract = typeof(IChatManagerCallBack))]
    public interface IChatManager {

        /// <summary>
        /// Sends a message to your current lobby chat (if you are in)
        /// </summary>
        /// <param name="username">The player sending the message</param>
        /// <param name="message">The message the player wants to send</param>
        /// <param name="lobbyCode">You current lobbyCode</param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(string username, Message message, string lobbyCode);

        /// <summary>
        /// Connects a player to a specific lobby chat
        /// </summary>
        /// <param name="username">The player that is connecting</param>
        /// <param name="lobbyCode">The desired lobby chat the player wants to join</param>
        [OperationContract(IsOneWay = true)]
        void ConnectToChat(string username, string lobbyCode);

        /// <summary>
        /// Disconnects a player from a lobby chat
        /// </summary>
        /// <param name="username">The player to be disconnected</param>
        /// <param name="lobbyCode">Player current lobby's code to be disconnected (if in)</param>
        [OperationContract(IsOneWay = true)]
        void LeaveChat(string username, string lobbyCode);

    }
    [ServiceContract]
    public interface IChatManagerCallBack {

        /// <summary>
        /// Callback that sends a Message to a player 
        /// </summary>
        /// <param name="message">Message that needs to be sent</param>
        [OperationContract]
        void BroadcastMessage(Message message);

    }
}
