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

        public int updateAccount(Profile profile) {
            throw new NotImplementedException();
        }
    }
}
