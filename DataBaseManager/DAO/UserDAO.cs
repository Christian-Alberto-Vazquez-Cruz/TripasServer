using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using DataBaseManager.Utils;


namespace DataBaseManager.DAO {
    public class UserDAO {
        public int addUserDAO(Perfil profile, Login user) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Login newUserLogin = new Login {
                        correo = user.correo,
                        contrasena = user.contrasena
                    };
                    db.Login.Add(newUserLogin);
                    db.SaveChanges();

                    Perfil newUserProfile = new Perfil {
                        nombre = profile.nombre,
                        puntaje = Constants.INITIAL_SCORE,
                        fotoRuta = Constants.INITIAL_PIC_PATH,
                        idPerfil = newUserLogin.idUsuario
                    };
                    db.Perfil.Add(newUserProfile);
                    db.SaveChanges();
                    operationStatus = Constants.SUCESS;
                }
            }
            catch (EntityException entityException) {
                Console.WriteLine("Error trying to register the user {0}", entityException.Message);
            }
            return operationStatus;
        }

    public int validateUserDAO(Login user) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var userExists = db.Login.Any(login => login.correo == user.correo && login.contrasena == user.contrasena);
                    if (userExists) {
                        operationStatus = Constants.FOUND_MATCH;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) { 
                Console.WriteLine("Error trying to validate user: {0}", entityException.Message);
            }
            return operationStatus;
        }

        public int updateUserProfileDAO(Perfil profile) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == profile.idPerfil);
                    if (existingProfile != null) {
                        existingProfile.nombre = profile.nombre;
                        existingProfile.fotoRuta = profile.fotoRuta;
                        db.SaveChanges();
                        operationStatus = Constants.SUCESS;
                    }
                    else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            }
            catch (EntityException entittyException) {
                Console.WriteLine("Error trying to update the user profile {0}, {1}", profile.nombre, entittyException.Message);
            }
            return operationStatus;
        }

        public Perfil getProfileByMail (String mail) {
            Perfil userProfile = null;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var login = db.Login.FirstOrDefault(l => l.correo == mail);
                    if (login != null) {
                        userProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == login.idUsuario);
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine("Error trying to get the Profile {0}", entityException.Message);
            }
            return userProfile;
        }
    }
}
