using DataBaseManager;
using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;

namespace TripasService.Services {
    public partial class TripasGameService : IFriendsManager {
        public int addFriend(int idProfile1, int idProfile2) {
            int result = FriendsDAO.strikeUpFriendshipDAO(idProfile1, idProfile2);
            return result;
        }
        public int deleteFriend(int idProfile1, int idProfile2) {
            int result = FriendsDAO.deleteFriendshipDAO(idProfile1, idProfile2);
            return result;
        }

        public List<Profile> getFriends(int idProfile) {
            List<Perfil> friendProfiles = FriendsDAO.getFriendsDAO(idProfile);
            List<Profile> friendList = new List<Profile>();

            foreach (var friend in friendProfiles) {
                Profile profile = new Profile() {
                    idProfile = friend.idPerfil,
                    userName = friend.nombre,
                    score = friend.puntaje,
                    picturePath = friend.fotoRuta //¿Are we going to show the profile pic? 
                };
                friendList.Add(profile);
            }
            return friendList;
        }

        //¿Not useful anymore?
        public int deleteFriendship(string userName1, string userName2) {
            int result = FriendsDAO.deleteFriendsDAO(userName1, userName2);
            return result;
        }
    }
}
    