using DataBaseManager.DAO;
using DataBaseManager.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasTests.ProxyTripas;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using static System.Net.Mime.MediaTypeNames;

namespace TripasTests.DAO {


    public class UserDAOShould : IClassFixture<DatabaseFixture> {

        [Fact]
        public void AddUser() {
            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                correo = "zS22011132@estudiantes.uv.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = "Pedrinho",
            };

            int userAdded = Constants.SUCCESSFUL_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.AddUserDAO(newPerfil, newLogin);

            Assert.Equal(userAdded, resultObtained);
        }


        [Fact]
        public void AddUserException() {
            DataBaseManager.Login newLoginEmpty = new DataBaseManager.Login();
            DataBaseManager.Perfil newPerfilEmpty = new DataBaseManager.Perfil();

            int operationFailed = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.AddUserDAO(newPerfilEmpty, newLoginEmpty);

            Assert.Equal(operationFailed, resultObtained);
        }

        [Fact]
        public void ValidateUser() {
            string mail = "virtualbox@hotmail.com.mx";
            string password = "MiContrasena1!";

            int userExists = Constants.FOUND_MATCH;
            int resultObtained = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, mail);

            Assert.Equal(userExists, resultObtained);
        }

        [Fact]
        public void ValidateUserNotFound() {
            string password = "Serpiente2!";
            string email = "j@gmail.com";

            int userDoesntExist = Constants.NO_MATCHES;
            int resultObtained = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, email);

            Assert.Equal(userDoesntExist, resultObtained);
        }


        [Fact]
        public void ValidateUserException() {
            string password = null;
            string mail = null;

            int exceptionResult = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, mail);

            Assert.Equal(exceptionResult, resultObtained);
        }

        [Fact]
        public void UpdateUserProfile() {
            int id = 4;
            string newPicPath = "/Images/Profiles/ImageProfile9.png";
            string newUsername = "Nombre";

            int userUpdated = Constants.SUCCESSFUL_OPERATION;
            int result = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(userUpdated, result);
        }

        [Fact]
        public void UpdateUserProfileNotFound() {
            int id = 115;
            string newPicPath = Constants.INITIAL_PIC_PATH;
            string newUsername = "MathT";

            int userNotFound = Constants.NO_MATCHES;
            int resultObtained = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(userNotFound, resultObtained);
        }

        [Fact]
        public void UpdateUserProfileException() {
            int id = 5;
            string newPicPath = Constants.INITIAL_PIC_PATH;
            string newUsername = "Mouse";

            int expectionResult = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(expectionResult, resultObtained);
        }

        [Fact]
        public void GetProfileByMail() {
            DataBaseManager.Perfil obtainedPerfil = UserDAO.GetProfileByMailDAO("a@gmail.com");

            Profile obtainedProfile = new Profile() {
                Username = obtainedPerfil.nombre,
            };

            Profile expectedProfile = new Profile() {
                Username = "Alambrito",
            };

            Assert.Equal(obtainedProfile.Username, expectedProfile.Username);
        }

        [Fact]
        public void GetProfileByMailNoMatches() {

            string mail = "jerusalenAl@gmail.com";

            DataBaseManager.Perfil obtainedPerfil = UserDAO.GetProfileByMailDAO(mail);

            Profile noMatchesProfile = new Profile() {
                IdProfile = Constants.NO_MATCHES
            };

            Profile obtainedProfile = new Profile() {
                IdProfile = obtainedPerfil.idPerfil
            };

            Assert.Equal(noMatchesProfile.IdProfile, obtainedProfile.IdProfile);
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
        public void GetProfileById() {
            string username = "Alambrito";

            int expectedId = 1;
            int resultObtained = UserDAO.GetProfileIdDAO(username);

            Assert.Equal(expectedId, resultObtained);
        }

        [Fact]
        public void GetProfileByIdNotFound() {
            string username = "JuanElsabueso";

            int idNoMatches = Constants.NO_MATCHES;
            int resultObtained = UserDAO.GetProfileIdDAO(username);

            Assert.Equal(idNoMatches, resultObtained);
        }

        [Fact]
        public void GetProfileByException() {
            string username = "Alambrito";

            int idFailedOperation = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.GetProfileIdDAO(username);

            Assert.Equal(resultObtained, idFailedOperation);
        }

        [Fact]
        public void GetPicPathByUsername() {
            string username = "Alambrito";

            string expectedPicPath = "/Images/Profiles/ImageProfile2.png";
            string resultObtained = UserDAO.GetPicPathByUsername(username);

            Assert.Equal(expectedPicPath, resultObtained);
        }

        [Fact]
        public void GetPicPathByUsernameNoMatches() {
            string username = "Conejo";

            string noMatchesPicPath = Constants.NO_MATCHES_STRING;
            string resultObtained = UserDAO.GetPicPathByUsername(username);

            Assert.Equal(noMatchesPicPath, resultObtained);
        }

        [Fact]
        public void GetPicPathByUsernameException() {
            string username = "Pablo";

            string failedPicPath = Constants.FAILED_OPERATION_STRING;
            string resultObtained = UserDAO.GetPicPathByUsername(username);

            Assert.Equal(failedPicPath, resultObtained);
        }

        [Fact]
        public void GetMailByUsername() {
            string username = "vbox";

            string expectedEmail = "virtualbox@hotmail.com.mx";
            string resultObtained = UserDAO.GetMailByUsername(username);

            Assert.Equal(expectedEmail, resultObtained);
        }

        [Fact]
        public void GetMailByUsernameNotFound() {
            string username = "EmilianoZapata";

            string expectedMail = Constants.NO_MATCHES_STRING;
            string resultObtained = UserDAO.GetMailByUsername(username);

            Assert.Equal(expectedMail, resultObtained);
        }

        [Fact]
        public void GetMailByUsernameException() {
            string username = "Pinguinela";

            string emailRegistered = Constants.FAILED_OPERATION_STRING;
            string resultObtained = UserDAO.GetMailByUsername(username);

            Assert.Equal(emailRegistered, resultObtained);
        }

        [Fact]
        public void IsEmailRegistered() {
            string mail = "zS22013636@estudiantes.uv.mx";

            int emailRegistered = Constants.FOUND_MATCH;
            int resultObtained = UserDAO.IsEmailRegisteredDAO(mail);

            Assert.Equal(emailRegistered, resultObtained);
        }

        [Fact]
        public void IsEmailNotRegistered() {
            string mail = "lkq@gmail.com";


            int emailNotRegistered = Constants.NO_MATCHES;
            int resultObtained = UserDAO.IsEmailRegisteredDAO(mail);

            Assert.Equal(emailNotRegistered, resultObtained);
        }

        [Fact]
        public void IsEmailRegisteredException() {
            string mail = "Pinguinela";


            int emailNotRegistered = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.IsEmailRegisteredDAO(mail);

            Assert.Equal(emailNotRegistered, resultObtained);
        }


        [Fact]
        public void IsUsernameRegistered() {
            string username = "Alambrito";

            int usernameRegistered = Constants.FOUND_MATCH;
            int resultObtained = UserDAO.IsNameRegisteredDAO(username);

            Assert.Equal(usernameRegistered, resultObtained);

        }

        [Fact]
        public void IsUsernameRegisteredNoMatches() {
            string username = "Sefirot";

            int usernameNotRegistered = Constants.NO_MATCHES;
            int resultObtained = UserDAO.IsNameRegisteredDAO(username);


            Assert.Equal(usernameNotRegistered, resultObtained);
        }

        [Fact]
        public void IsUsernameRegisteredException() {
            string username = "Pinguinela";

            int operationFailed = Constants.FAILED_OPERATION;
            int resultObtained = UserDAO.IsNameRegisteredDAO(username);

            Assert.Equal(operationFailed, resultObtained);
        }

        [Fact]
        public void UpdateLoginPassword() {
            string mail = "zS22013636@estudiantes.uv.mx";
            string newPassword = "NuevaContrasena1!";

            int operationSuccessful = Constants.SUCCESSFUL_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.UpdateLoginPasswordDAO(mail, newPassword);

            Assert.Equal(operationSuccessful, resultObtained);
        }

        [Fact]
        public void UpdateLoginPasswordNotFound() {
            string mail = "garnachasDemoniacas.uv.mx";
            string newPassword = "NuevaContrasena1!";

            int noMatches = Constants.NO_MATCHES;
            int resultObtained = DataBaseManager.DAO.UserDAO.UpdateLoginPasswordDAO(mail, newPassword);

            Assert.Equal(noMatches, resultObtained);
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
        public void DeleteAccount() {
            string email = "Pinguinela@hotmail.com.mx";


            int expectedOperationStatus = Constants.SUCCESSFUL_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.DeleteAccountDAO(email);

            Assert.Equal(expectedOperationStatus, resultObtained);
        }

        [Fact]
        public void DeleteAccountNotFound() {
            string email = "garnachasDemoniacas.uv.mx";

            int expectedOperationStatus = Constants.NO_MATCHES;
            int resultObtained = DataBaseManager.DAO.UserDAO.DeleteAccountDAO(email);

            Assert.Equal(expectedOperationStatus, resultObtained);
        }

        [Fact]
        public void DeleteAccountException() {
            string email = "gamesa@gmail.com";  

            int expectedOperationStatus = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.DeleteAccountDAO(email);

            Assert.Equal(expectedOperationStatus, resultObtained);
        }

        [Fact]
        public void UpdatePlayerScore() {
            string username = "Pablo";
            int additionalPoints = 10;

            int expected = Constants.SUCCESSFUL_OPERATION;
            int resultObtained = UserDAO.UpdatePlayerScore(username, additionalPoints);

            Assert.Equal(expected, resultObtained);
        }

        [Fact]
        public void UpdatePlayerScoreNoMatches() {
            string username = "garnachasDemoniacas";
            int additionalPoints = 10;

            int result = UserDAO.UpdatePlayerScore(username, additionalPoints);

            int expected = Constants.NO_MATCHES;
            Assert.Equal(expected, result);
        }


        [Fact]
        public void UpdatePlayerScoreException() {
            string username = "vbox"; 
            int additionalPoints = 10;

            int expected = Constants.FAILED_OPERATION;
            int result = UserDAO.UpdatePlayerScore(username, additionalPoints);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsFriendAlreadyAdded() {
            int idProfile1 = 2002;  
            int idProfile2 = 3;  

            int expectedOperationResult = Constants.SUCCESSFUL_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);

            Assert.Equal(expectedOperationResult, resultObtained);
        }

        [Fact]
        public void IsFriendAlreadyAddedNoMatches() {
            int idProfile1 = 1;  
            int idProfile2 = 99; 

            int expectedOperationResult = Constants.NO_MATCHES;
            int resultObtained = DataBaseManager.DAO.UserDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);

            Assert.Equal(expectedOperationResult, resultObtained);
        }

        [Fact]
        public void IsFriendAlreadyAddedException() {
            int idProfile1 = 2002;
            int idProfile2 = 3; 

            int expectedOperationResult = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);

            Assert.Equal(expectedOperationResult, resultObtained);
        }

    }

    public class DatabaseFixture : IDisposable {

        public DatabaseFixture() {

            DataBaseManager.Login testLogin = new DataBaseManager.Login() {
                correo = "virtualbox@hotmail.com.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testProfile = new DataBaseManager.Perfil() {
                nombre = "vbox",
            };


            DataBaseManager.Login testLogin2 = new DataBaseManager.Login() {
                correo = "Pablito@hotmail.com.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testProfile2 = new DataBaseManager.Perfil() {
                nombre = "Pablo",
            };

            UserDAO.AddUserDAO(testProfile2, testLogin2);

            DataBaseManager.Login testLogin3 = new DataBaseManager.Login() {
                correo = "Pinguinela@hotmail.com.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testProfile3 = new DataBaseManager.Perfil() {
                nombre = "Pinguinela",
            };

            DataBaseManager.Login testLogin4 = new DataBaseManager.Login() {
                correo = "gamesa@gmail.com",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testProfile4 = new DataBaseManager.Perfil() {
                nombre = "galletasGamesa",
            };

            UserDAO.AddUserDAO(testProfile, testLogin);
            UserDAO.AddUserDAO(testProfile2, testLogin2);
            UserDAO.AddUserDAO(testProfile3, testLogin3);
            UserDAO.AddUserDAO(testProfile4, testLogin4);
        }

        public void Dispose() {
            UserDAO.DeleteAccountDAO("virtualbox.com.mx");
            UserDAO.DeleteAccountDAO("zS22011132@estudiantes.uv.mx");
            UserDAO.DeleteAccountDAO("Pablito@hotmail.com.mx");

        }
    }
}