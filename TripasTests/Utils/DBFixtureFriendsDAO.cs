using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasTests.Utils {
    public class DBFixtureFriendsDAO : IDisposable {
        public DBFixtureFriendsDAO() {
            FriendsDAO.StrikeUpFriendshipDAO(8216, 8220);
            FriendsDAO.StrikeUpFriendshipDAO(8216, 8217);
            FriendsDAO.StrikeUpFriendshipDAO(1, 8216);
            FriendsDAO.StrikeUpFriendshipDAO(6048, 8216);
        }
        public void Dispose() {
            FriendsDAO.DeleteFriendshipDAO(8216, 8220);
            FriendsDAO.DeleteFriendshipDAO(8216, 8217); 
            FriendsDAO.DeleteFriendshipDAO(1, 8216);
            FriendsDAO.DeleteFriendshipDAO(6048, 8216);

        }
    }
}
