using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasTests.Utils {
    public class DBFixtureDAOException : IDisposable {
        public void Dispose() {
            FriendsDAO.DeleteFriendshipDAO(8218, 8219);

        }
    }
}
