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
                    // Registrar login del nuevo usuario
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
                logger.LogError($"DbEntityValidationException: Error trying to register user: {profile.nombre}. Exception: {dbEntityValidationException.Message}", dbEntityValidationException);
            } catch (DbUpdateException dbUpdateException) {
                logger.LogError($"DbUpdateException: Error updating the database while registering user: {profile.nombre}. Exception: {dbUpdateException.Message}", dbUpdateException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while processing user registration for {profile.nombre}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while registering user {profile.nombre}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error trying to validate user with email: {mail}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while querying database to validate user with email: {mail}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while validating user with email: {mail}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error trying to update user profile with id: {idProfile}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while querying the database to update user profile with id: {idProfile}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while updating user profile with id: {idProfile}. Exception: {generalException.Message}", generalException);
            }
            return operationStatus;
        }

        public static Perfil GetProfileByMailDAO(string mail) {
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
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Error while retrieving profile by email: {mail}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while querying the database to retrieve profile by email: {mail}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while retrieving profile by email: {mail}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error while retrieving profile ID for username: {username}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error occurred while querying the database to retrieve profile ID for username: {username}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while retrieving profile ID for username: {username}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error while retrieving profile picture path for username: {username}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to retrieve profile picture path for username: {username}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while retrieving profile picture path for username: {username}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error while retrieving email for username: {username}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to retrieve email for username: {username}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error occurred while retrieving email for username: {username}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error while checking if email {mail} is already registered. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to check email registration for {mail}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error while checking email registration for {mail}. Exception: {generalException.Message}", generalException);
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
                logger.LogError($"SqlException: Error while checking if the username {username} is already registered. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to check username registration for {username}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error while checking username registration for {username}. Exception: {generalException.Message}", generalException);
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
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Error while updating the password for email {mail}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to update password for email {mail}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error while updating password for email {mail}. Exception: {generalException.Message}", generalException);
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
                            db.Perfil.Remove(userProfile);  // Eliminar el perfil
                        }
                        db.Login.Remove(userLogin);  // Eliminar el login
                        db.SaveChanges();  // Guardar los cambios en la base de datos
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Error while trying to delete the user with email {email}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to delete the user with email {email}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error while deleting the user with email {email}. Exception: {generalException.Message}", generalException);
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
                        db.SaveChanges();  // Guardar los cambios en la base de datos
                        operationStatus = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationStatus = Constants.NO_MATCHES;  // No se encontró el perfil
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Error while updating score for player {username}. Exception: {sqlException.Message}", sqlException);
            } catch (EntityException entityException) {
                logger.LogError($"EntityException: Error while accessing the database to update score for player {username}. Exception: {entityException.Message}", entityException);
            } catch (Exception generalException) {
                logger.LogError($"Exception: Unexpected error while updating score for player {username}. Exception: {generalException.Message}", generalException);
            }
            return operationStatus;
        }

        public static int IsFriendAlreadyAddedDAO(int idProfile1, int idProfile2) {
            LoggerManager logger = new LoggerManager(typeof(UserDAO));
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var existingFriendship = db.Amistad.FirstOrDefault(a =>
                        (a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2) ||
                        (a.idJugadorUno == idProfile2 && a.idJugadorDos == idProfile1));  // También se considera la amistad en sentido inverso
                    if (existingFriendship != null) {
                        operationResult = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationResult = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                logger.LogError($"SqlException: Error while checking friendship status for profiles {idProfile1} and {idProfile2}. Exception: {sqlException.Message}", sqlException);
            } catch (Exception exception) {
                logger.LogError($"Exception: Unexpected error while checking friendship status for profiles {idProfile1} and {idProfile2}. Exception: {exception.Message}", exception);
            }
            return operationResult;
        }
    }
}