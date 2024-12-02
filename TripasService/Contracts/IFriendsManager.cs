using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IFriendsManager {
        /// <summary>
        /// Creates a "local" friendship between a player and another
        /// </summary>
        /// <param name="idProfile1">Id of the player that wants to add a friend/param>
        /// <param name="idProfile2">Id of the player that will be added to the friednlist</param>
        /// <returns>Returns 1 in success and -1 in failure</returns>
        [OperationContract]
        int AddFriend(int idProfile1, int idProfile2);

        /// <summary>
        /// Deletes the "local" friendship between a player and another
        /// </summary>
        /// <param name="idProfile1">Id of the player that wants to delete a friend/param>
        /// <param name="idProfile2">Id of the player that will be deleted from the friendlist</param>
        /// <returns>Returns 1 in success, -1 in failure and -2 if the friendship wasn't found </returns>
        [OperationContract]
        int DeleteFriend(int idProfile1, int idProfile2);


        /// <summary>
        /// Deletes the "local" friendship between a player and another
        /// </summary>
        /// <param name="idProfile">Id of the player that wants to retrieve his friends/param>
        /// <returns>Returns an empty List<Profile> if no friends, or a list<Profile> with username and status</returns>
        [OperationContract] 
        List<Profile> GetFriends(int idProfile);

    }
}
