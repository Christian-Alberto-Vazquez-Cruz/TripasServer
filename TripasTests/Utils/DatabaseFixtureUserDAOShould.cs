using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasTests.Utils {
    public class DatabaseFixtureUserDAOShould : IDisposable {
        private readonly List<string> _addedEmails = new List<string>();
        public DatabaseFixtureUserDAOShould() {
            AddTestUser("virtualbox@hotmail.com.mx", "vbox");
            AddTestUser("Pablito@hotmail.com.mx", "Pablo");
            AddTestUser("Pinguinela@hotmail.com.mx", "Pinguinela");
            AddTestUser("gamesa@gmail.com", "galletasGamesa");
        }

        private void AddTestUser(string email, string username) {
            DataBaseManager.Login newLogin = new DataBaseManager.Login() {
                correo = email,
                contrasena = "MiContrasena1!"
            };

            DataBaseManager.Perfil newPerfil = new DataBaseManager.Perfil() {
                nombre = username
            };

            UserDAO.AddUserDAO(newPerfil, newLogin);
            _addedEmails.Add(email);
        }

        public void Dispose() {
            foreach (string email in _addedEmails) {
                UserDAO.DeleteAccountDAO(email);
            }
        }
    }
}