using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;


namespace TripasService.Contracts {
    [ServiceContract]
    public interface ILeaderboardManager {

        /// <summary>
        /// Retrieves the 10 highests scores players
        /// </summary>
        /// <param name="username">User in friend list to invite</param>
        /// <param name="code">Lobby code</param>
        /// <returns>Returns a list of Profiles with username and score, if there are no players, the list will be empty</returns>

        [OperationContract]
        List<Profile> GetHighestScores();
    }
}
