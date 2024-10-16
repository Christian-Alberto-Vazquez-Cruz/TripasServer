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
            UserDAO dao = new UserDAO();

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
             contrasena = user.password,
                correo = user.mail
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = profile.userName,
            };

            int insertionResult = dao.addUser(newPerfil, newLogin);
            return insertionResult;
        }

        public Profile getProfile(string email) {
            throw new NotImplementedException();
        }

        public int updateProfile(Profile profile) {
            UserDAO dao = new UserDAO();

            DataBaseManager.Perfil perfil = new DataBaseManager.Perfil() {
                idPerfil = profile.idProfile,
                nombre = profile.userName,
                fotoRuta = profile.picturePath,
            };
            int result = dao.updateUserProfile(perfil);
            return result;
        }

        public int verifyLogin(LoginUser user) {
            UserDAO dao = new UserDAO();

            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                contrasena = user.password,
                correo = user.mail
            };

            int result = dao.validateUser(newLogin);
            return result;
        } 
    }
}