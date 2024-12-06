using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager {

        // <summary>
        /// Registers a trace in a match that will be soon sent to the the opposite player. Also scores the trace.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <param name="trace">The trace containing the score update.</param>
        /// <returns>Returns true if the trace was successfully registered, false otherwise.</returns>

        [OperationContract]
        bool RegisterTrace(string matchCode, Trace trace);

        /// <summary>
        /// Registers a callback for a player in a match.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <param name="username">The player's username.</param>
        /// <returns>Returns true if the callback was successfully registered, false otherwise.</returns>
        [OperationContract]
        bool RegisterPlayerCallback(string matchCode, string username);


        /// <summary>
        /// Retrieves the list of nodes for a given match.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <returns>A list of nodes in the match.</returns>
        [OperationContract]
        List<Node> GetNodes(string matchCode);

        /// <summary>
        /// Retrieves the pairs of nodes in a match.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <returns>A dictionary where each key is a node  and the value its pair a string.</returns>
        [OperationContract]
        Dictionary<string, string> GetNodePairs(string matchCode);

        /// <summary>
        /// Ends the turn of the current player in the match.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <param name="userName">The username of the player whose turn is being ended.</param>
        /// <returns>Returns true if the turn was successfully ended, false otherwise.</returns>
        [OperationContract]
        bool EndTurn(string matchCode, string userName);

        /// <summary>
        /// Retrieves the username of the player whose turn it currently is in the match.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <returns>The username of the player whose turn it is.</returns>
        [OperationContract]
        string GetCurrentTurn(string matchCode);

        /// <summary>
        /// Ends the match and notifies players of the result. This operation is one-way.
        /// </summary>
        /// <param name="matchCode">The code identifying the match to be ended.</param>
        [OperationContract(IsOneWay = true)]
        void EndMatch(string matchCode);

        /// <summary>
        /// Notifies that a player has left the match. This operation is one-way.
        /// </summary>
        /// <param name="matchCode">The code identifying the match.</param>
        /// <param name="username">The username of the player who left the match.</param>
        [OperationContract(IsOneWay = true)]
        void LeaveMatch(string matchCode, string username);

    }

    [ServiceContract]
    public interface IMatchManagerCallback {

        /// <summary>
        /// Notifies that a drawn trace to a player.
        /// </summary>
        /// <param name="trace">The trace that was received.</param>
        [OperationContract(IsOneWay = true)]
        void TraceReceived(Trace trace);

        /// <summary>
        /// Notifies the player that it is now their turn.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyYourTurn();

        /// <summary>
        /// Notifies the player that it is not their turn.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyNotYourTurn();

        /// <summary>
        /// Notifies the player that they lost the match.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyYouLost();

        /// <summary>
        /// Notifies the player that they won the match.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyYouWon();

        /// <summary>
        /// Notifies the player that the match ended in a draw.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyDraw();

        /// <summary>
        /// Notifies the player that another player has left the match.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerLeft();

    }
}