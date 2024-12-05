using System;
using System.Linq;
using DataBaseManager.Utils;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace DataBaseManager.DAO {
    public static class UserDAO {
        public static int AddUserDAO(Perfil profile, Login newLogin) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Login newUserLogin = new Login {
                        correo = newLogin.correo,
                        contrasena = newLogin.contrasena
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
            } catch (DbEntityValidationException dbEntityValidationException) {
                logger.LogError(dbEntityValidationException);
                Console.WriteLine($"Error trying to register the user: {profile.nombre}, {dbEntityValidationException.Message}");
            } catch (DbUpdateException dbUpdateException) {
                logger.LogError(dbUpdateException);
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return operationStatus;
        }

        public static int ValidateUserDAO(string password, string mail) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
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
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error trying to validate user: {mail}, {sqlException.Message}");
                //operationStatus = Constants.FAILED_OPERATION;
            } catch (EntityException entityException) {
                logger.LogError(entityException);
                //operationStatus = Constants.FAILED_OPERATION;
            }
            return operationStatus;
        }

        public static int UpdateUserProfileDAO(int idProfile, string newUsername, string newPicPath) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Perfil existingProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == idProfile);
                    if (existingProfile != null) {
                        existingProfile.nombre = newUsername;
                        existingProfile.fotoRuta = newPicPath;
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error trying to update the user profile with {idProfile} id, {sqlException.Message}");
            } catch (EntityException entittyException) {
                logger.LogError(entittyException);
            }
            return operationStatus;
        }

        public static Perfil GetProfileByMailDAO(String mail) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            Perfil userProfile = new Perfil {
                idPerfil = Constants.FAILED_OPERATION  // Asignar por defecto el valor de operación fallida
            };
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Login userLogin = db.Login.FirstOrDefault(login => login.correo == mail);

                    if (userLogin != null) {
                        Perfil foundProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == userLogin.idUsuario);
                        if (foundProfile != null) {
                            userProfile = foundProfile;
                        } else {
                            userProfile.idPerfil = Constants.NO_MATCHES;
                        }
                    } else {
                        userProfile.idPerfil = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlExcepion) {
                logger.LogError(sqlExcepion);
                Console.WriteLine($"Error trying to get the Profile with {mail} mail, {sqlExcepion.Message}");
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return userProfile;
        }

        public static int GetProfileIdDAO(string username) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int profileId = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    profileId = db.Perfil.Where(u => u.nombre == username).Select(u => u.idPerfil).FirstOrDefault();
                    if (profileId == 0) {
                        profileId = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine("Error trying to get the user id with username {0}", sqlException.Message);
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return profileId;
        }

        public static string GetPicPathByUsername(string username) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            string picPath = Constants.FAILED_OPERATION_STRING;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Perfil userProfile = db.Perfil.FirstOrDefault(perfil => perfil.nombre == username);
                    if (userProfile != null) {
                        picPath = userProfile.fotoRuta;
                    } else {
                        picPath = Constants.NO_MATCHES_STRING;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error trying to get the profile picture path for username {username}: {sqlException.Message}");
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }

            return picPath;
        }

        public static string GetMailByUsername(string username) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            string mail = Constants.FAILED_OPERATION_STRING;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Perfil userProfile = db.Perfil.FirstOrDefault(perfil => perfil.nombre == username);
                    if (userProfile != null) {
                        Login userLogin = db.Login.FirstOrDefault(login => login.idUsuario == userProfile.idPerfil);
                        if (userLogin != null) {
                            mail = userLogin.correo;
                        } else {
                            mail = Constants.NO_MATCHES_STRING;
                        }
                    } else {
                        mail = Constants.NO_MATCHES_STRING;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error trying to get the mail for username {username}: {sqlException.Message}");
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return mail;
        }

        public static int IsEmailRegisteredDAO(string mail) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationStatus = Constants.FAILED_OPERATION;

            try {
                using (tripasEntities db = new tripasEntities()) {
                    if (db.Login.Any(login => login.correo == mail)) {
                        operationStatus = Constants.FOUND_MATCH;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error checking if the email is already registered {sqlException.Message}");
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return operationStatus;
        }

        public static int IsNameRegisteredDAO(string username) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    if (db.Perfil.Any(profile => profile.nombre == username)) {
                        operationStatus = Constants.FOUND_MATCH;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error checking if the email is already registered {sqlException.Message}");
            } catch (EntityException exception) {
                logger.LogError(exception);
            }
            return operationStatus;
        }

        public static int UpdateLoginPasswordDAO(string mail, string newPassword) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
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
            } catch (SqlException entityException) {
                logger.LogError(entityException);
                Console.WriteLine($"Error trying to update the login password with {mail} mail, {entityException.Message}");
            } catch (EntityException exception) {
                logger.LogError(exception);
            }
            return operationStatus;
        }

        public static int DeleteAccountDAO(string email) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Login userLogin = db.Login.FirstOrDefault(login => login.correo == email);
                    if (userLogin == null) {
                        operationStatus = Constants.NO_MATCHES;
                    } else {
                        Perfil userProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == userLogin.idUsuario);
                        if (userProfile != null) {
                            db.Perfil.Remove(userProfile);
                        }
                        db.Login.Remove(userLogin);
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error trying to delete the user with email {email}, {sqlException.Message}");
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return operationStatus;
        }

        public static int UpdatePlayerScore(string username, int additionalPoints) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Perfil userProfile = db.Perfil.FirstOrDefault(p => p.nombre == username);
                    if (userProfile != null) {
                        userProfile.puntaje += additionalPoints;
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
                Console.WriteLine($"Error trying to update {username} score: {sqlException.Message}");
            } catch (EntityException entityException) {
                logger.LogError(entityException);
            }
            return operationStatus;
        }

        public static int IsFriendAlreadyAddedDAO(int idProfile1, int idProfile2) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingFriendship = db.Amistad.FirstOrDefault(a =>
                        a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2);

                    if (existingFriendship != null) {
                        operationResult = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationResult = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError(sqlException);
            } catch (Exception exception) {
                logger.LogError(exception);
            }
            return operationResult;
        }


    }
}