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
        public int CreateAccount(LoginUser user, Profile profile) {

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                contrasena = user.password,
                correo = user.mail
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = profile.userName,
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

        public Profile GetProfileByMail(string mail) {
            Perfil profileDB = UserDAO.GetProfileByMailDAO(mail);
            Profile profile = new Profile() {
                idProfile = profileDB.idPerfil,
                userName = profileDB.nombre,
                picturePath = profileDB.fotoRuta,
            };
            return profile;
        }

        public int GetProfileId(string userName) {
            int result = UserDAO.GetProfileIdDAO(userName);
            if (result == Constants.NO_MATCHES) {
                ProfileNotFoundFault profileNotFound = new ProfileNotFoundFault();
                profileNotFound.errorMessage = $"Couldn't find a profile that matches {userName} name";
                throw new FaultException<ProfileNotFoundFault>(profileNotFound);
                //throw new FaultException<ProfileNotFoundFault>(new ProfileNotFoundFault($"Couldn't find a profile that matches {userName} name"));
            }
            return result;
        }

        public bool IsEmailRegistered(string email) {
            bool isRegistered = UserDAO.IsEmailRegisteredDAO(email);
            return isRegistered;
        }

        public int UpdateProfileName(int idProfile, string newProfileName) {
            int result = UserDAO.UpdateProfileNameDAO(idProfile, newProfileName);
            return result;
        }

        public int UpdateProfilePic(int idProfile, string newProfilePic) {
            int result = UserDAO.UpdateProfilePicDAO(idProfile, newProfilePic);
            return result;
        }

    }
}