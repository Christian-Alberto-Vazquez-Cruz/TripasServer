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
    public partial class TripasGameService : IUserManager {
        public int createAccount(LoginUser user, Profile profile) {

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                contrasena = user.password,
                correo = user.mail
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = profile.userName,
            };

            int insertionResult = UserDAO.addUserDAO(newPerfil, newLogin);
            return insertionResult;
        }

        public int updateProfile(int idProfile, string newUsername, string newPic) {
            int result = UserDAO.updateUserProfileDAO(idProfile, newUsername, newPic);
            return result;
        }

        public int verifyLogin(string mail, string password) {
            int result = UserDAO.validateUserDAO(password, mail);
            return result;
        }

        public Profile getProfileByMail(string mail) {
            Perfil profileDB = UserDAO.getProfileByMailDAO(mail);
            Profile profile = new Profile() {
                idProfile = profileDB.idPerfil,
                userName = profileDB.nombre,
                picturePath = profileDB.fotoRuta,
            };
            return profile;
        }

        public int getProfileId(string userName) {
            int result = UserDAO.getProfileIdDAO(userName);
            if (result == Constants.NO_MATCHES) {
                ProfileNotFoundFault profileNotFound = new ProfileNotFoundFault();
                profileNotFound.errorMessage = $"Couldn't find a profile that matches {userName} name";
                throw new FaultException<ProfileNotFoundFault>(profileNotFound);
                //throw new FaultException<ProfileNotFoundFault>(new ProfileNotFoundFault($"Couldn't find a profile that matches {userName} name"));
            }
            return result;
        }

        public bool isEmailRegistered(string email) {
            bool isRegistered = UserDAO.isEmailRegisteredDAO(email);
            return isRegistered;
        }

        public int updateProfileName(int idProfile, string newProfileName) {
            int result = UserDAO.updateProfileNameDAO(idProfile, newProfileName);
            return result;
        }

        public int updateProfilePic(int idProfile, string newProfilePic) {
            int result = UserDAO.updateProfilePicDAO(idProfile, newProfilePic);
            return result;
        }

    }
}