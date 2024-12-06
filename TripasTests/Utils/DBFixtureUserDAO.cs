using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasTests.Utils {
    public class DBFixtureUserDAO : IDisposable {
        private readonly List<string> _addedEmails = new List<string>();
        public DBFixtureUserDAO() {
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
            _addedEmails.Add("zS22011132@estudiantes.uv.mx");
        }

        public void Dispose() {
            foreach (string email in _addedEmails) {
                UserDAO.DeleteAccountDAO(email);
            }
        }
    }
}