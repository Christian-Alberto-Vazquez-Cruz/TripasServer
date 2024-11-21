﻿using DataBaseManager.DAO;
using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace TripasTests.DAO {
    public class UserDAOShould {

        [Fact]
        public void AddUser() {
            DataBaseManager.Login testLogin = new DataBaseManager.Login() {
                correo = "zS22013636@estudiantes.uv.mx",
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testPerfil = new DataBaseManager.Perfil() {
                nombre = "Pedrinho",
                puntaje = 0,
                fotoRuta = Constants.INITIAL_PIC_PATH
            };

            int userAdded = Constants.SUCCESSFUL_OPERATION;
            int result = DataBaseManager.DAO.UserDAO.AddUserDAO(testPerfil, testLogin);

            Assert.Equal(userAdded, result);

            DataBaseManager.DAO.UserDAO.DeleteUserDAO(testLogin.correo);
        }

    // [FACT] ¿Debería haber un caso de prueba por valores nulos? ¿Debería haber un caso de prueba con un valor nulo y otro no?
    // [FACT] Un caso de prueba por un error con la BD, ¿Cómo?

        [Fact]
        public void ValidateUser() {
            //¿Debería realizarse la inserción previamente?
            string password = "4fc5e3e3a50b4b10a0b71cc5053b813d44bec408fd6c2f02eb520c5401d5c3a7";
            string email = "a@gmail.com";

            int userExists = Constants.FOUND_MATCH;
            int result = DataBaseManager.DAO.UserDAO.ValidateUserDAO(password, email);

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

        //[FACT] ¿Flujo donde lanza EntityException?

        [Fact]
        public void UpdateUserProfile() {
            //¿Agregar usuario?
            int id = 4;
            string newPicPath = "";
            string newUsername = "Nombre";

            int userUpdated = Constants.SUCCESSFUL_OPERATION;
            int result = DataBaseManager.DAO.UserDAO.UpdateUserProfileDAO(id, newUsername, newPicPath);

            Assert.Equal(userUpdated, result);  

            //¿Devolver usuario a sus valores originales?
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

        /*[Fact]
        public void GetProfileByMail() {

            string testEmail = "test@gmail.com";
            DataBaseManager.Login testLogin = new DataBaseManager.Login {
                idUsuario = 2000,
                correo = testEmail,
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil testPerfil = new DataBaseManager.Perfil {
                idPerfil = 2000,
                nombre = "TestUser",
                puntaje = 0,
                fotoRuta = Constants.INITIAL_PIC_PATH
            };

            DataBaseManager.DAO.UserDAO.AddUserDAO(testPerfil, testLogin);

            DataBaseManager.Perfil expectedProfile = testPerfil;
            DataBaseManager.Perfil result = DataBaseManager.DAO.UserDAO.GetProfileByMailDAO(testEmail);

            Assert.Equal(expectedProfile, result);

            DataBaseManager.DAO.UserDAO.DeleteUserDAO(testLogin.correo);
        }*/

        [Fact] //¿Se debería insertar un nuevo Login y luego borrarlo? 
        public void IsEmailRegistered() {
            string testEmail = "a@gmail.com";
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
        public void IsEmailRegisteredNull() {
            string testEmail = null;
            int result = UserDAO.IsEmailRegisteredDAO(testEmail);

            int failedOperation = Constants.FAILED_OPERATION;
            Assert.Equal(failedOperation, result);
        }

        //[Fact] IsEmailRegisteredEntityException ¿Qué hago sin Mocks?

        [Fact]
        public void IsUsernameRegistered() {
            string testUsername = "zetazeta";
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

        [Fact]
        public void IsUsernameRegisteredNull() { //En el anterior se hizo un if (string.IsNullOrEmpty(mail)) ¿Aquí igual o no? 
                                                 // ¿Y si se maneja en el método que expone el contrato y no en el DAO?
            string testUsername = null;
            int result = UserDAO.IsNameRegistered(testUsername);

            Assert.Equal(Constants.FAILED_OPERATION, result);
        }

        //[Fact] IsUsernameRegisteredEntityException ¿Qué hago sin Mocks?
    }
}