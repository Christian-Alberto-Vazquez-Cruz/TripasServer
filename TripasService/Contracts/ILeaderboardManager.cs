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
        /// <returns>Returns an empty List<Profile> if there are no players, or a list<Profile> with username and score</returns>
        [OperationContract]
        List<Profile> GetHighestScores();
    }
}
