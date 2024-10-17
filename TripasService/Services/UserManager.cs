using DataBaseManager;
using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using TripasService.Utils;



namespace TripasService.Services {
    [ServiceBehavior]
    public partial class TripasGameService :IUserManager {
        public int createAccount(LoginUser user, Profile profile) {
            UserDAO dao = new UserDAO();

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
             contrasena = user.password,
                correo = user.mail
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = profile.userName,
            };

            int insertionResult = dao.addUserDAO(newPerfil, newLogin);
            return insertionResult;
        }

        public Profile getProfile(String mail, String password) {
            UserDAO dao = new UserDAO();
            DataBaseManager.Login loginDetails = new DataBaseManager.Login() {
                correo = mail,
                contrasena = password
            };

            int loginStatus = dao.validateUserDAO(loginDetails);
            if (loginStatus == Constants.FOUND_MATCH) {
                Perfil profileDB = dao.getProfileByMail(mail);

                Profile profile = new Profile() {
                    idProfile = profileDB.idPerfil,
                    userName = profileDB.nombre,
                    picturePath = profileDB.fotoRuta,
                };
                return profile;
            }
            else {
                return null;
            }
        }

        public int updateProfile(Profile profile) {
            UserDAO dao = new UserDAO();

            DataBaseManager.Perfil perfil = new DataBaseManager.Perfil() {
                idPerfil = profile.idProfile,
                nombre = profile.userName,
                fotoRuta = profile.picturePath,
            };
            int result = dao.updateUserProfileDAO(perfil);
            return result;
        }

        // Decidir si utilizar este o getProfile que hace todo
          public int verifyLogin(LoginUser user) {
            UserDAO dao = new UserDAO();

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                contrasena = user.password,
                correo = user.mail
            };

            int result = dao.validateUserDAO(newLogin);
            return result;
        } 
    }
}