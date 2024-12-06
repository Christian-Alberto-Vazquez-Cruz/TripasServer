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
        /// <param name="idProfile1">Id of the player that wants to add a friend</param>
        /// <param name="idProfile2">Id of the player that will be added to the friednlist</param>
        /// <returns>Returns 1 in success and -1 in failure</returns>

        [OperationContract]
        int AddFriend(int idProfile1, int idProfile2);

        /// <summary>
        /// Deletes the "local" friendship between a player and another
        /// </summary>
        /// <param name="idProfile1">Id of the player that wants to delete a friend</param>
        /// <param name="idProfile2">Id of the player that will be deleted from the friend relationship</param>
        /// <returns>Returns 1 in success and -1 in failure</returns>
        [OperationContract]
        int DeleteFriend(int idProfile1, int idProfile2);

        /// <summary>
        /// Retrieves a Profile list with the content of the player's friends
        /// </summary>
        /// <param name="idProfile">Id of the player that will have his/her friendlist retrieved</param>
        /// <returns>Returns 1 in success and -1 in failure</returns>
        [OperationContract] 
        List<Profile> GetFriends(int idProfile);

        /// <summary>
        /// Verifies is a player had another player already added as a friend
        /// </summary>
        /// <param name="idProfile1">Id of the player whose friend association will be consulted</param>
        /// <param name="idProfile2">Id of the player you want to know if is already added </param>
        /// <returns>Returns 1 if it was already added, -2 if it han't been added and -1 in failure</returns>

        [OperationContract]
        int IsFriendAlreadyAdded(int idProfile1, int idProfile2); //FALTA PROBAR

    }
}
