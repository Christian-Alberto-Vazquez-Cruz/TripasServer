using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
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
                    operationStatus = Constants.SUCCESS;
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to register the user with {user.correo} mail, {profile.idPerfil} idProfile. {entityException.Message}");
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
                Console.WriteLine($"Error trying to validate user: {user.idUsuario} {user.correo}, {entityException.Message}");
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
                        operationStatus = Constants.SUCCESS;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entittyException) {
                Console.WriteLine($"Error trying to update the user profile {profile.nombre}, {entittyException.Message}");
            }
            return operationStatus;
        }

        public Perfil getProfileByMail(String mail) {
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

        public int getProfileIdDAO(string userName) {
            int profileId = Constants.NO_MATCHES;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    profileId = db.Perfil.Where(perfil => perfil.nombre == userName).Select(perfil => perfil.idPerfil).FirstOrDefault();
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to get the user id with {userName} username, {entityException.Message}");
            }
            return profileId;
        }

        public bool isEmailRegisteredDAO(string mail) {
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

        public int updateProfileNameDAO(int idProfile, string newProfileName) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == idProfile);
                    if (existingProfile != null) {
                        existingProfile.nombre = newProfileName;
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESS;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to update the user profile NAME with {idProfile} id, {entityException.Message}");
            }
            return operationStatus;
        }

        public int updateProfilePicDAO(int idProfile, string newProfilePic) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == idProfile);
                    if (existingProfile != null) {
                        existingProfile.fotoRuta = newProfilePic;
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESS;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to update the user profile PIC with {idProfile} id, {entityException.Message}");
            }
            return operationStatus;
        }

        public int updateLoginPasswordDAO(string mail, string newPassword) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingLogin = db.Login.FirstOrDefault(login => login.correo == mail);
                    if (existingLogin != null) {
                        existingLogin.contrasena = newPassword;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to update the login password with {mail} mail, {entityException.Message}");
            }
            return operationStatus;
        }

        public List<Perfil> getHighestScoresDAO() {
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
    }
}
