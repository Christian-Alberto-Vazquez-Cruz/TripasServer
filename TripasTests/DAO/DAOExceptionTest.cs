using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DataBaseManager;
using TripasTests.Utils;
using DataBaseManager.Utils;
using TripasTests.ProxyTripas;

namespace TripasTests.DAO {
    public class DAOExceptionTest: IClassFixture<DBFixtureDAOException> {

        [Fact]
        public void ValidateUserException() {
            string password = null;
            string mail = null;

            int expectedResult = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, mail);

            Assert.Equal(expectedResult, resultObtained);
        }


        [Fact]
        public void UpdateUserProfileException() {
            int id = 5;
            string newPicPath = Constants.INITIAL_PIC_PATH;
            string newUsername = "Mouse";

            int expectedResult = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(expectedResult, resultObtained);
        }

        [Fact]
        public void GetProfileByMailDatabaseFailure() {
            string testEmail = "zS22013636@mail.com";

            DataBaseManager.Perfil obtainedPerfil = UserDAO.GetProfileByMailDAO(testEmail);

            Profile failedRetrievalProfile = new Profile() {
                IdProfile = Constants.FAILED_OPERATION
            };

            Profile obtainedProfile = new Profile() {
                IdProfile = obtainedPerfil.idPerfil
            };

            Assert.Equal(failedRetrievalProfile.IdProfile, obtainedPerfil.idPerfil);
        }

        [Fact]
        public void GetProfileByIdException() {
            string username = "Alambrito";

            int idFailedOperation = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.GetProfileIdDAO(username);

            Assert.Equal(resultObtained, idFailedOperation);
        }


        [Fact]
        public void GetPicPathByUsernameException() {
            string username = "Pablo";

            string failedPicPath = Constants.FAILED_OPERATION_STRING;
            string resultObtained = UserDAO.GetPicPathByUsername(username);

            Assert.Equal(failedPicPath, resultObtained);
        }


        [Fact]
        public void GetMailByUsernameException() {
            string username = "Pinguinela";

            string emailRegistered = Constants.FAILED_OPERATION_STRING;
            string resultObtained = UserDAO.GetMailByUsername(username);

            Assert.Equal(emailRegistered, resultObtained);
        }

        [Fact]
        public void IsEmailRegisteredException() {
            string mail = "Pinguinela";


            int emailNotRegistered = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.IsEmailRegisteredDAO(mail);

            Assert.Equal(emailNotRegistered, resultObtained);
        }

        [Fact]
        public void IsUsernameRegisteredException() {
            string username = "Pinguinela";

            int operationFailed = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.IsNameRegisteredDAO(username);

            Assert.Equal(operationFailed, resultObtained);
        }

        [Fact]
        public void UpdateLoginPasswordExceptionException() {
            string mail = "zS22013636@estudiantes.uv.mx";
            string newPassword = "NuevaContrasena1!";

            int failedOperation = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.UpdateLoginPasswordDAO(mail, newPassword);

            Assert.Equal(failedOperation, resultObtained);
        }

        [Fact]
        public void DeleteAccountException() {
            string email = "gamesa@gmail.com";

            int expectedOperationStatus = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.DeleteAccountDAO(email);

            Assert.Equal(expectedOperationStatus, resultObtained);
        }

        [Fact]
        public void UpdatePlayerScoreException() {
            string username = "vbox";
            int additionalPoints = 10;

            int expected = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.UpdatePlayerScore(username, additionalPoints);

            Assert.Equal(expected, resultObtained);
        }


        [Fact]
        public void StrikeUpFriendshipDAOException() {

            int idProfile1 = 8218;
            int idProfile2 = 8219;

            var expected = Constants.FAILED_OPERATION;
            var resultObtained = FriendsDAO.StrikeUpFriendshipDAO(idProfile1, idProfile2);

            Assert.Equal(expected, resultObtained);
        }

        [Fact]
        public void DeleteFriendshipDAOException() {
            int idProfile1 = 8218;
            int idProfile2 = 8219;

            var expectedResult = Constants.FAILED_OPERATION;
            int obtainedResult = FriendsDAO.DeleteFriendshipDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }

        [Fact]
        public void GetFriendsDAOException() {
            int idProfile = 1;

            int expectedProfileId = Constants.FAILED_OPERATION; 
            List<Perfil> friends = FriendsDAO.GetFriendsDAO(idProfile);

            Assert.Contains(friends, friend => friend.idPerfil == expectedProfileId);
        }

        [Fact]
        public void IsFriendAlreadyAddedException() {
            int idProfile1 = 6048;
            int idProfile2 = 8218;

            int expectedResult = Constants.FAILED_OPERATION;
            int obtainedResult = FriendsDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);

            Assert.Equal(expectedResult, obtainedResult);
        }

        [Fact]
        public void GetHighestScoresDAOException() {

            int expectedProfileId = Constants.FAILED_OPERATION;
            List<Perfil> leaderboardPlayers = LeaderboardDAO.GetHighestScoresDAO();

            Assert.Contains(leaderboardPlayers, player => player.idPerfil == expectedProfileId);
        }
    }
}
