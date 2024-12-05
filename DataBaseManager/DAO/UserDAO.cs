using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
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
        public static int AddUserDAO(Perfil profile, Login newLogin) {
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
                Console.WriteLine($"Error trying to register the user: {profile.nombre}, {dbEntityValidationException.Message}");
            } catch (DbUpdateException dbUpdateException) {
                //LOGGEAR
            } catch (EntityException entityException) {
                //LOGGEAR
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
            } catch (SqlException sqlException) {
                Console.WriteLine($"Error trying to validate user: {mail}, {sqlException.Message}");
                //operationStatus = Constants.FAILED_OPERATION;
            } catch (EntityException entityException) {
                //LOGGEAR
                //operationStatus = Constants.FAILED_OPERATION;
            }
            return operationStatus;
        }

        public static int UpdateUserProfileDAO(int idProfile, string newUsername, string newPicPath) {
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
            } catch (EntityException entittyException) {
                Console.WriteLine($"Error trying to update the user profile with {idProfile} id, {entittyException.Message}");
            }
            return operationStatus;
        }

        public static Perfil GetProfileByMailDAO(String mail) {
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
                //LOGGEAR
                Console.WriteLine($"Error trying to get the Profile with {mail} mail, {sqlExcepion.Message}");
            } catch (EntityException entityException) {
                //LOGGEAR
            }
            return userProfile;
        }

        public static int GetProfileIdDAO(string username) {
            int profileId = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    profileId = db.Perfil.Where(u => u.nombre == username).Select(u => u.idPerfil).FirstOrDefault();
                    if (profileId == 0) {
                        profileId = Constants.NO_MATCHES;
                    }
                }
            } catch (SqlException sqlException) {
                //LOG
                Console.WriteLine("Error trying to get the user id with username {0}", sqlException.Message);
            } catch (EntityException entityException) {
                //LOG
            }
            return profileId;
        }

        public static string GetPicPathByUsername(string username) {
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
                //LOGGEAR
               Console.WriteLine($"Error trying to get the profile picture path for username {username}: {sqlException.Message}");
            } catch (EntityException entityException) {
               //LOGGEAR
            }
             
            return picPath;
        }

        public static string GetMailByUsername(string username) {
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
                Console.WriteLine($"Error trying to get the mail for username {username}: {sqlException.Message}");
            } catch (EntityException entityException) {

            }
            return mail;
        }

        public static int IsEmailRegisteredDAO(string mail) {
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
                Console.WriteLine($"Error checking if the email is already registered {sqlException.Message}");
            } catch (EntityException entityException) {

            }
            return operationStatus;
        }

        public static int IsNameRegisteredDAO(string username) {
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
                Console.WriteLine($"Error checking if the email is already registered {sqlException.Message}");
            } catch (EntityException exception) {
            }
            return operationStatus;
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
            } catch (SqlException entityException) {
                Console.WriteLine($"Error trying to update the login password with {mail} mail, {entityException.Message}");
            } catch (EntityException exception) {

            }
            return operationStatus;
        }

        public static int DeleteAccountDAO(string email) {
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
            } catch (SqlException sqlException) {
                Console.WriteLine($"Error trying to delete the user with email {email}, {sqlException.Message}");
            } 
            return operationStatus;
        }

        public static void UpdatePlayerScore(string userName, int additionalPoints) {
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var userProfile = db.Perfil.FirstOrDefault(p => p.nombre == userName);
                    if (userProfile != null) {
                        userProfile.puntaje = userProfile.puntaje + additionalPoints;
                        db.SaveChanges();
                        Console.WriteLine($"Puntos actualizados para {userName}: {userProfile.puntaje}");
                    }
                }   
            } catch (EntityException ex) {
                Console.WriteLine($"Error al actualizar los puntos del jugador {userName}: {ex.Message}");
            }
        }

        /* public static int AddUserWithSpecificIdDAO(Perfil profile, Login user) {
             int operationStatus = Constants.FAILED_OPERATION;
             try {
                 using (tripasEntities db = new tripasEntities()) {
                     if (db.Login.Any(login   => login.idUsuario == user.idUsuario) ||
                         db.Perfil.Any(userProfile => userProfile.idPerfil == userProfile.idPerfil)) {
                         Console.WriteLine($"El ID de usuario {user.idUsuario} ya existe.");
                         return Constants.FAILED_OPERATION;
                     }

                     db.Configuration.AutoDetectChangesEnabled = false;

                     Login newUserLogin = new Login {
                         idUsuario = user.idUsuario,
                         correo = user.correo,
                         contrasena = user.contrasena
                     };
                     db.Login.Add(newUserLogin);
                     db.SaveChanges();

                     Perfil newUserProfile = new Perfil {
                         idPerfil = user.idUsuario, 
                         nombre = profile.nombre,
                         puntaje = Constants.INITIAL_SCORE,
                         fotoRuta = Constants.INITIAL_PIC_PATH
                     };
                     db.Perfil.Add(newUserProfile);
                     db.SaveChanges();

                     db.Configuration.AutoDetectChangesEnabled = true;
                     operationStatus = Constants.SUCCESSFUL_OPERATION;
                 }
             } catch (EntityException entityException) {
                 Console.WriteLine($"Error trying to register the user with {user.correo} mail and specific ID {user.idUsuario}. {entityException.Message}");
             }
             return operationStatus;
         }*/
        public static int IsFriendAlreadyAddedDAO(int idProfile1, int idProfile2) {
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
                //Loggear
            } catch (Exception ex) {
                //loggear
            }
            return operationResult;
        }
    }
}