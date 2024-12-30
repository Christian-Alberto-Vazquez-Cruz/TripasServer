using DataBaseManager;
using DataBaseManager.DAO;
using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasTests.Utils;
using Xunit;

namespace TripasTests.DAO {
    public class FriendsDAOSTest : IClassFixture<DBFixtureFriendsDAO> {

        [Fact]
        public void StrikeUpFriendshipDAO() {
            int idProfile1 = 8216;
            int idProfile2 = 8217;

            int expectedResult = Constants.SUCCESSFUL_OPERATION;
            int obtainedResult = FriendsDAO.StrikeUpFriendshipDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }

        [Fact]
        public void DeleteFriendshipDAO() {
            int idProfile1 = 8216;
            int idProfile2 = 8217;

            var expectedResult = Constants.SUCCESSFUL_OPERATION;
            int obtainedResult = FriendsDAO.DeleteFriendshipDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }


        [Fact]
        public void DeleteFriendshipDAONoMatches() {
            int idProfile1 = 20000;
            int idProfile2 = 20002;

            var expectedResult = Constants.NO_MATCHES;
            int obtainedResult = FriendsDAO.DeleteFriendshipDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }

        [Fact]
        public void GetFriendsDAO() {
            var expectedCount = 1;
            int idProfile = 8216;

            List<Perfil> friends = FriendsDAO.GetFriendsDAO(idProfile);

            Assert.Equal(expectedCount, friends.Count);
        }

        [Fact]
        public void GetFriendsDAOEmpty() {
            var expectedCount = 0;
            int idProfile = 8225;

            FriendsDAO.GetFriendsDAO(idProfile);
            List<Perfil> friends = FriendsDAO.GetFriendsDAO(idProfile);

            Assert.Equal(expectedCount, friends.Count);
        }

        [Fact]
        public void IsFriendAlreadyAddedDAO() {
            int idProfile1 = 6048;
            int idProfile2 = 8216;

            int expectedResult = Constants.SUCCESSFUL_OPERATION;
            int obtainedResult = FriendsDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }

        [Fact]
        public void IsFriendAlreadyAddedDAONoMatches() {
            int idProfile1 = 6048;
            int idProfile2 = 8217;

            int expectedResult = Constants.NO_MATCHES;
            int obtainedResult = FriendsDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }
    }
}
