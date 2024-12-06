using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface ILobbyBrowser {

        /// <summary>
        /// Retrieves a list of currently available lobbies.
        /// </summary>
        /// <returns>A list of lobbies that are currently available.</returns>
        [OperationContract]
        List<Lobby> GetAvailableLobbies();

        /// <summary>
        /// Allows a guest player to join an existing lobby using its code.
        /// </summary>
        /// <param name="code">The unique code of the lobby to join.</param>
        /// <param name="guest">The profile of the guest player.</param>
        /// <returns>Returns true if the player successfully joined the lobby; false otherwise.</returns>
        [OperationContract]
        bool JoinLobby(string code, Profile guest);

        /// <summary>
        /// Creates a new lobby for a game with a specified number of nodes and host.
        /// </summary>
        /// <param name="gameName">The name of the game being played in the lobby.</param>
        /// <param name="nodeCount">The number of nodes for the game.</param>
        /// <param name="host">The profile of the player hosting the lobby.</param>
        /// <returns>The unique code for the newly created lobby, or a failure message if the creation failed.</returns>
        [OperationContract]
        string CreateLobby(string gameName, int nodeCount, Profile host);

        /// <summary>
        /// Retrieves a lobby by its unique code.
        /// </summary>
        /// <param name="code">The unique code of the lobby to retrieve.</param>
        /// <returns>The lobby object if found, or a failed operation message if not found.</returns>
        [OperationContract]
        Lobby GetLobbyByCode(string code);
    }
}