using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IFriendsManager {
        [OperationContract]
        int AddFriend(int idProfile1, int idProfile2);
        //Not useful anymore?
        [OperationContract]
        int DeleteFriend(int idProfile1, int idProfile2);
        [OperationContract]
        int DeleteFriendship(string userName1, string userName2);
        [OperationContract] 
        List<Profile> GetFriends(int idProfile);

    }
}
