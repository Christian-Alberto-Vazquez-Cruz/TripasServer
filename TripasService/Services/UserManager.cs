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
using TripasService.Enums;

namespace TripasService.Services {
    [ServiceBehavior]
    public partial class TripasGameService : IUserManager {
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
        public int verifyLogin(string mail, string password) {
            UserDAO dao = new UserDAO();
            int result = dao.validateUserDAO(password, mail);
            return result;
        }

        public Profile getProfileByMail(string mail) {
            UserDAO dao = new UserDAO();
            Perfil profileDB = dao.getProfileByMailDAO(mail);
            Profile profile = new Profile() {
                idProfile = profileDB.idPerfil,
                userName = profileDB.nombre,
                picturePath = profileDB.fotoRuta,
                status = GaneEnums.PlayerStatus.Online
            };
            return profile;
        }

        public int getProfileId(string userName) {
            UserDAO dao = new UserDAO();
            int result = dao.getProfileIdDAO(userName);
            if (result == Constants.NO_MATCHES) {
                ProfileNotFoundFault profileNotFound = new ProfileNotFoundFault();
                profileNotFound.errorMessage = $"Couldn't find a profile that matches {userName} name";
                throw new FaultException<ProfileNotFoundFault>(profileNotFound);
                //throw new FaultException<ProfileNotFoundFault>(new ProfileNotFoundFault($"Couldn't find a profile that matches {userName} name"));
            }
            return result;
        }

        public bool isEmailRegistered(string email) {
            UserDAO dao = new UserDAO();
            bool isRegistered = dao.isEmailRegisteredDAO(email);
            return isRegistered;
        }

        public int updateProfileName(int idProfile, string newProfileName) {
            UserDAO dao = new UserDAO();
            int result = dao.updateProfileNameDAO(idProfile, newProfileName);
            return result;
        }

        public int updateProfilePic(int idProfile, string newProfilePic) {
            UserDAO dao = new UserDAO();
            int result = dao.updateProfilePicDAO(idProfile, newProfilePic);
            return result;
        }

    }
}