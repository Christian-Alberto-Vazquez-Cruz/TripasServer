using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.DAO {
    public static class FriendsDAO {
        public static int StrikeUpFriendshipDAO(int idProfile1, int idProfile2) {
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Amistad newFriendship = new Amistad() {
                        idJugadorUno = idProfile1,
                        idJugadorDos = idProfile2
                    };

                    db.Amistad.Add(newFriendship);
                    db.SaveChanges();
                    operationResult = Constants.SUCCESSFUL_OPERATION;
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to register the friendship {entityException.Message}");
            }
            return operationResult;
        }

        public static int DeleteFriendshipDAO(int idProfile1, int idProfile2) {
            int operationResult = Constants.FAILED_OPERATION;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var friendshipToDelete = db.Amistad.FirstOrDefault(a =>
                    a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2);

                    if (friendshipToDelete != null) {
                        db.Amistad.Remove(friendshipToDelete);
                        db.SaveChanges();
                        operationResult = Constants.SUCCESSFUL_OPERATION;
                    } else {
                        operationResult = Constants.NO_MATCHES;
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine(entityException.Message);
            }
            return operationResult;
        }

        public static List<Perfil> GetFriendsDAO(int idProfile) {
            List<Perfil> friendList = new List<Perfil>();
            Perfil operationFailed = new Perfil();
            operationFailed.idPerfil = Constants.FAILED_OPERATION;

            try {
                using (tripasEntities db = new tripasEntities()) {
                    var friends = (from friendship in db.Amistad
                                   where friendship.idJugadorUno == idProfile
                                   join profile in db.Perfil on friendship.idJugadorDos equals profile.idPerfil
                                   select profile).ToList();
                    friendList = friends;
                }
            } catch (EntityException entityException) {
                friendList.Add(operationFailed);
                Console.WriteLine($"Error trying to retrieve the friend list {entityException.Message}");
            }
            return friendList;
        }

        public static int IsFriendAlreadyAddedDAO(int idProfile1, int idProfile2) {
            int operationResult = Constants.FAILED_OPERATION;

            try {
                using (tripasEntities db = new tripasEntities()) {
                    // Verificar si existe una relación de amistad con idJugadorUno = idProfile1 y idJugadorDos = idProfile2
                    var existingFriendship = db.Amistad.FirstOrDefault(a =>
                        a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2);

                    if (existingFriendship != null) {
                        operationResult = Constants.SUCCESSFUL_OPERATION; // Ya son amigos
                    } else {
                        operationResult = Constants.NO_MATCHES; // No son amigos
                    }
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to verify friendship: {entityException.Message}");
            }

            return operationResult;
        }
    }
}