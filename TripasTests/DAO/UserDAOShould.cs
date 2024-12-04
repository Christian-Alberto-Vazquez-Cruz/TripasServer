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


    public class UserDAOShould: IClassFixture<DatabaseFixture> {

        [Fact]
        public void AddUser() {
            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                correo = "zS22011132@estudiantes.uv.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = "Pedrinho",
                fotoRuta = Constants.INITIAL_PIC_PATH,
            };

            int userAdded = Constants.SUCCESSFUL_OPERATION;
            int result = DataBaseManager.DAO.UserDAO.AddUserDAO(newPerfil, newLogin);

            Assert.Equal(userAdded, result);
        }


        [Fact]
        public void AddUserException() {
            DataBaseManager.Login newLoginEmpty = new DataBaseManager.Login();
            DataBaseManager.Perfil newPerfilEmpty = new DataBaseManager.Perfil();


            int operationFailed = Constants.FAILED_OPERATION;
            int resultObtained = DataBaseManager.DAO.UserDAO.AddUserDAO(newPerfilEmpty, newLoginEmpty);

            Assert.Equal(operationFailed, resultObtained);
        }

        // [FACT] ¿Debería haber un caso de prueba por valores nulos? ¿Debería haber un caso de prueba con un valor nulo y otro no?
        // [FACT] Un caso de prueba por un error con la BD, ¿Cómo?

        [Fact]
        public void ValidateUser() {
            string mail = "test@hotmail.com.mx";
            string password = "MiContrasena1!";

            int userExists = Constants.FOUND_MATCH;
            int result = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, mail);

            Assert.Equal(userExists, result);
        }

        [Fact]
        public void ValidateUserNotFound() {
            string password = "4fc5e3e3a50b4b10a0b71cc5053b813d44bec408fd6c2f02eb520c5401d5c3a7";
            string email = "j@gmail.com";

            int userDoesntExist = Constants.NO_MATCHES;
            int result = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, email);

            Assert.Equal(userDoesntExist, result);    
        }


        [Fact]
        public void ValidateUserException() {
            string password = null;
            string mail = null;

            int exceptionResult = Constants.FAILED_OPERATION;
            int result = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, mail);

            Assert.Equal(exceptionResult, result);
        }

        [Fact]
        public void UpdateUserProfile() {
            //¿Agregar usuario?
            int id = 4;
            string newPicPath = "/Images/Profiles/ImageProfile9.png";
            string newUsername = "Nombre";

            int userUpdated = Constants.SUCCESSFUL_OPERATION;
            int result = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(userUpdated, result);
        }

        [Fact]
        public void UpdateUserProfileNotFound() {
            //¿Eliminar usuario si existe?
            int id = 115;
            string newPicPath = Constants.INITIAL_PIC_PATH;
            string newUsername = "Nombre";

            int userNotFound = Constants.NO_MATCHES;
            int result = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(userNotFound, result);
        }

        [Fact]
        public void GetProfileByMail() {

            DataBaseManager.Perfil obtainedPerfil = UserDAO.GetProfileByMailDAO("a@gmail.com");

            Profile obtainedProfile = new Profile() {
                IdProfile = obtainedPerfil.idPerfil,
                Username = obtainedPerfil.nombre,
                Score = obtainedPerfil.puntaje,
                PicturePath = obtainedPerfil.fotoRuta,
                status = GameEnumsPlayerStatus.Offline
            };


            Profile expectedProfile = new Profile() {
                IdProfile = 1,
                Username = "Alambrito",
                Score = 0,
                PicturePath = "/Images/Profiles/ImageProfile9.png",
                status = GameEnumsPlayerStatus.Offline
            };

            Assert.Equal(obtainedProfile.Username, expectedProfile.Username);
        }

        [Fact] 
        public void IsEmailRegistered() {
            string testEmail = "zS22013636@estudiantes.uv.mx";
            int result = UserDAO.IsEmailRegisteredDAO(testEmail);

            int emailRegistered = Constants.FOUND_MATCH;
            Assert.Equal(emailRegistered, result);
        }

        [Fact]
        public void IsEmailNotRegistered() {
            string testEmail = "lkq@gmail.com";
            int result = UserDAO.IsEmailRegisteredDAO(testEmail);

            int emailNotRegistered = Constants.NO_MATCHES;
            Assert.Equal(emailNotRegistered, result);
        }


        [Fact]
        public void IsUsernameRegistered() {
            string testUsername = "test";
            int result = UserDAO.IsNameRegistered(testUsername);

            int usernameRegistered = Constants.FOUND_MATCH;
            Assert.Equal(usernameRegistered, result);
           
        }

        [Fact]
        public void IsUsernameNotRegistered() {
            string testUsername = "Sefirot";
            int result = UserDAO.IsNameRegistered(testUsername);

            int usernameNotRegistered = Constants.NO_MATCHES;
            Assert.Equal(usernameNotRegistered, result);
        }

        //[Fact] IsUsernameRegisteredEntityException ¿Qué hago sin Mocks?

    }

    public class DatabaseFixture : IDisposable {

        public DatabaseFixture() {

            DataBaseManager.Login testLogin = new DataBaseManager.Login() {
                correo = "test@hotmail.com.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testProfile = new DataBaseManager.Perfil() {
                nombre = "test",
                puntaje = 0,
                fotoRuta = Constants.INITIAL_PIC_PATH
            };

            UserDAO.AddUserDAO(testProfile, testLogin);

            DataBaseManager.Login testLogin2 = new DataBaseManager.Login() {
                correo = "Pablito@hotmail.com.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testProfile2 = new DataBaseManager.Perfil() {
                nombre = "Pablo",
                puntaje = 0,
                fotoRuta = Constants.INITIAL_PIC_PATH
            };

            UserDAO.AddUserDAO(testProfile2, testLogin2);

        }
        public void Dispose() {
            UserDAO.DeleteAccountDAO("test@hotmail.com.mx");
            UserDAO.DeleteAccountDAO("zS22011132@estudiantes.uv.mx");
            UserDAO.DeleteAccountDAO("Pablito@hotmail.com.mx");
        }
    }
}