using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using DataBaseManager.Utils;


namespace DataBaseManager.DAO {
    public static class UserDAO {
        public static int AddUserDAO(Perfil profile, Login user) {
            int operationStatus = Constants.FAILED_OPERATION;
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
                    operationStatus = Constants.SUCCESSFUL_OPERATION;
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to register the user with {user.correo} mail, {profile.idPerfil} idProfile. {entityException.Message}");
            }
            return operationStatus;
        }

        public static int ValidateUserDAO(string password, string mail) {
            int operationStatus = Constants.FAILED_OPERATION;

            try {
                using (tripasEntities db = new tripasEntities()) {
                    bool userExists = db.Login.Any(login => login.correo == mail && login.contrasena == password);

                    if (userExists) {
                        operationStatus = Constants.FOUND_MATCH;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to validate user: {mail}, {entityException.Message}");
            }

            return operationStatus;
        }

        public static int UpdateUserProfileDAO(int idProfile, string newUsername, string newPic) {
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == idProfile);
                    if (existingProfile != null) {
                        existingProfile.nombre = newUsername;
                        existingProfile.fotoRuta = newPic;
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entittyException) {
                Console.WriteLine($"Error trying to update the user profile with {idProfile} id, {entittyException.Message}");
            }
            return operationStatus;
        }

        public static Perfil GetProfileByMailDAO(String mail) {
            Perfil userProfile = null;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var userLogin = db.Login.FirstOrDefault(login => login.correo == mail);
                    if (userLogin != null) {
                        userProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == userLogin.idUsuario);
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to get the Profile with {mail} mail, {entityException.Message}");
            }
            return userProfile;
        }

        public static int GetProfileIdDAO(string userName) {
            int profileId = Constants.NO_MATCHES;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    profileId = db.Perfil.Where(u => u.nombre == userName).Select(u => u.idPerfil).FirstOrDefault();
                }
            } catch (EntityException entityException) {
                Console.WriteLine("Error trying to get the user id with username {0}", entityException.Message);
            }
            return profileId;
        }

        public static bool IsEmailRegisteredDAO(string mail) {
            bool emailExists = false;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    emailExists = db.Login.Any(login => login.correo == mail);
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error checking if the email is already registered {entityException.Message}");
            }
            return emailExists;
        }

        public static int UpdateLoginPasswordDAO(string mail, string newPassword) {
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingLogin = db.Login.FirstOrDefault(login => login.correo == mail);
                    if (existingLogin != null) {
                        existingLogin.contrasena = newPassword;
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to update the login password with {mail} mail, {entityException.Message}");
            }
            return operationStatus;
        }

        public static List<Perfil> GetHighestScoresDAO() {
            List<Perfil> bestPlayersList = new List<Perfil>();
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var profiles = db.Perfil.OrderByDescending(perfil => perfil.puntaje).Take(Constants.HOW_MANY_SCORES).ToList();
                    bestPlayersList = profiles;
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to retrieve the highest score players {entityException.Message}");
            }
            return bestPlayersList;
        }

        public static int DeleteUserDAO(string email) {
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Login userLogin = db.Login.FirstOrDefault(login => login.correo == email);
                    if (userLogin != null) {
                        Perfil userProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == userLogin.idUsuario);

                        if (userProfile != null) {
                            db.Perfil.Remove(userProfile);
                        }
                        db.Login.Remove(userLogin);
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to delete the user with email {email}, {entityException.Message}");
            }
            return operationStatus;
        }
    }
}
