using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.DAO {
    public class FriendsDAO {
        public int strikeUpFriendshipDAO(int idProfile1, int idProfile2) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Amistad newFriendship = new Amistad() {
                        idJugadorUno = idProfile1,
                        idJugadorDos = idProfile2
                    };

                    db.Amistad.Add(newFriendship);
                    db.SaveChanges();
                    operationStatus = Constants.SUCCESS;
                }
            }
            catch (EntityException entityException) {
                Console.WriteLine($"Error trying to register the friendship {entityException.Message}");
            }
            return operationStatus;
        }

        //Not useful anymore?
        public int deleteFriendshipDAO(int idProfile1, int idProfile2) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var friendshipToDelete = db.Amistad.FirstOrDefault(a =>
                    a.idJugadorUno == idProfile1 && a.idJugadorDos == idProfile2);

                    if (friendshipToDelete != null) {
                        // Si se encontró la amistad, eliminarla
                        db.Amistad.Remove(friendshipToDelete);
                        db.SaveChanges();
                        operationStatus = Constants.SUCCESS;
                    }
                }
            }
            catch (EntityException entityException) {
                Console.WriteLine(entityException.Message);
            }
            return operationStatus;
        }

        public int deleteFriendsDAO(string userName1, string userName2) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var profile1 = db.Perfil.FirstOrDefault(profile => profile.nombre == userName1);
                    var profile2 = db.Perfil.FirstOrDefault(profile => profile.nombre == userName2);
                    if (profile1 != null && profile2 != null) {
                        var friendshipToDelete = db.Amistad.FirstOrDefault(perfil =>
                         perfil.idJugadorUno == profile1.idPerfil && perfil.idJugadorDos == profile2.idPerfil);

                        if (friendshipToDelete != null) {
                            db.Amistad.Remove(friendshipToDelete);
                            db.SaveChanges();
                            operationStatus = Constants.SUCCESS;
                        }
                    }
                }
            }
            catch (EntityException entityException) {
                Console.WriteLine($"Unable to delete friendship, {entityException.Message}");
            }
            return operationStatus;
        }



        public List<Perfil> getFriendsDAO(int idProfile) {
            List<Perfil> friendList = new List<Perfil>();
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var friends = (from friendship in db.Amistad
                                   where friendship.idJugadorUno == idProfile
                                   join profile in db.Perfil on friendship.idJugadorDos equals profile.idPerfil
                                   select profile).ToList();
                    friendList = friends;
                }
            }
            catch (EntityException entityException) {
                Console.WriteLine($"Error trying to retrieve the friend list {entityException.Message}");
            }
            return friendList;
        }
    }
}