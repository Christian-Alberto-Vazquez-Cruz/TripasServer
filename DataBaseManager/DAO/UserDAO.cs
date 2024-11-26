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

        //Comprueba si Login no es nulo, después si Perfil no es nulo
        public static Perfil GetProfileByMailDAO(String mail) {
            Perfil userProfile = new Perfil {
                idPerfil = Constants.NO_MATCHES
            };

            try {
                using (tripasEntities db = new tripasEntities()) {
                    Login userLogin = db.Login.FirstOrDefault(login => login.correo == mail);
                    if (userLogin != null) {
                        Perfil foundProfile = db.Perfil.FirstOrDefault(perfil => perfil.idPerfil == userLogin.idUsuario);
                        if (foundProfile != null) {
                            userProfile = foundProfile;
                        }
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

        //AQUÍ VERIFICAR QUÉ HACER CON PICPATH. ¿Puede ser nulo o asignar una por defecto? No tiene sentido regresar ToString();
        public static string GetPicPathByUsername(string username) {
            string picPath = Constants.FAILED_OPERATION.ToString();
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Perfil userProfile = db.Perfil.FirstOrDefault(perfil => perfil.nombre == username);

                    if (userProfile != null) {
                        picPath = userProfile.fotoRuta;
                    } else {
                        picPath = Constants.NO_MATCHES.ToString(); 
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to get the profile picture path for username {username}: {entityException.Message}");
            }

            return picPath;
        }

        public static string GetMailByUsername(string username) {
            string mail = "";
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Perfil userProfile = db.Perfil.FirstOrDefault(perfil => perfil.nombre == username);

                    if (userProfile != null) {
                        Login userLogin = db.Login.FirstOrDefault(login => login.idUsuario == userProfile.idPerfil);
                        if (userLogin != null) {
                            mail = userLogin.correo;
                        } 
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to get the mail for username {username}: {entityException.Message}");
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
            } catch (EntityException entityException) {
                Console.WriteLine($"Error checking if the email is already registered {entityException.Message}");
            }
            return operationStatus;
        }

        public static int IsNameRegistered(string username) {
            int operationStatus = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    if (db.Perfil.Any(profile => profile.nombre == username)) {
                        operationStatus = Constants.FOUND_MATCH;
                    } else {
                        operationStatus = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error checking if the email is already registered {entityException.Message}");
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
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to update the login password with {mail} mail, {entityException.Message}");
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
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to delete the user with email {email}, {entityException.Message}");
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

        public static int AddUserWithSpecificIdDAO(Perfil profile, Login user) {
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
        }
    }
}