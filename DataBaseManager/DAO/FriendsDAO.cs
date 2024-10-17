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
                    operationStatus = Constants.SUCESS;
                }
            } catch (EntityException entityException) {
                Console.WriteLine("Error trying to register the friendship {0}", entityException.Message);
            }
            return operationStatus;
        }

        public int deleteFriendshipDAO(int idProfile1, int idProfile2) {
            int operationStatus = Constants.FAILED;
            try {
                using (tripasEntities db = new tripasEntities()) {
                    Amistad friendshipToDelete = new Amistad() {
                        idJugadorUno = idProfile1,
                        idJugadorDos = idProfile2,
                    };

                    db.Amistad.Remove(friendshipToDelete);
                    db.SaveChanges();
                    operationStatus = Constants.SUCESS;
                }
            } catch (EntityException entityException) {
                Console.WriteLine(entityException.Message);
            }
            return operationStatus;
        }

        public List<Perfil> getFriendsDAO (int idProfile) {
            List<Perfil> friendList = new List<Perfil>();
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var friends = (from friendship in db.Amistad
                                   where friendship.idJugadorUno == idProfile
                                   join profile in db.Perfil on friendship.idJugadorDos equals profile.idPerfil
                                   select profile).ToList();
                    friendList = friends;
                }
            } catch (EntityException entityException) {
                Console.WriteLine("Error trying to retrieve the friend list {0}", entityException.Message);
            }
            return friendList;
        }
    }
}
