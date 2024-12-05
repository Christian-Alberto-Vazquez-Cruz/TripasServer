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
using TripasService.Logic;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;


namespace TripasService.Services {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public partial class TripasGameService : IUserManager {
        public int CreateAccount(LoginUser newUser, Profile newProfile) {

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                contrasena = newUser.Password,
                correo = newUser.Mail
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = newProfile.Username,
                puntaje = Constants.INITIAL_SCORE,
                fotoRuta = Constants.DEFAULT_PICPATH
            };

            int insertionResult = UserDAO.AddUserDAO(newPerfil, newLogin);
            return insertionResult;
        }

        public int UpdateProfile(int idProfile, string newUsername, string newPic) {
            int result = UserDAO.UpdateUserProfileDAO(idProfile, newUsername, newPic);
            return result;
        }

        public int VerifyLogin(string mail, string password) {
            int result = UserDAO.ValidateUserDAO(password, mail);
            return result;
        }

        public Profile GetProfileByMail(string email) {
            Perfil profileDB = UserDAO.GetProfileByMailDAO(email);
            Profile profile = new Profile() {
                IdProfile = profileDB.idPerfil,
                Username = profileDB.nombre,
                PicturePath = profileDB.fotoRuta,
            };
            return profile;
        }
        public int GetProfileId(string userName) {
            int result = UserDAO.GetProfileIdDAO(userName);
            return result;
        }

        public int IsEmailRegistered(string email) {
            int isRegistered = UserDAO.IsEmailRegisteredDAO(email);
            return isRegistered;
        }

        public int IsNameRegistered(string username) {
            int isRegistered = Constants.FAILED_OPERATION;
            if (!string.IsNullOrEmpty(username)) {
               isRegistered = UserDAO.IsNameRegisteredDAO(username);
            }
            return isRegistered;
        }

        public string GetPicPath(string username) {
            string result = UserDAO.GetPicPathByUsername(username);
            return result;
        }
    }
}