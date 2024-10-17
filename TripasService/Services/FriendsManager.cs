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
            FriendsDAO friendsDAO = new FriendsDAO();   
            int result = friendsDAO.strikeUpFriendshipDAO(idProfile1, idProfile2);
            return result;
        }

        public int deleteFriend(int idProfile1, int idProfile2) {
            FriendsDAO friendsDAO = new FriendsDAO();
            int result = friendsDAO.deleteFriendshipDAO(idProfile1, idProfile2);
            return result;
        }

        public List<Profile> getFriends(int idProfile) {
            FriendsDAO friendsDAO = new FriendsDAO();
            List<Perfil> friendProfiles= friendsDAO.getFriendsDAO(idProfile);
            List<Profile> friendList = new List<Profile>();

            foreach (var friend in friendProfiles) {
                Profile profile = new Profile() {
                    idProfile = friend.idPerfil,
                    userName = friend.nombre
                    //picturePath = friend.fotoRuta ¿Are we going to show the profile pic? 
                };
                friendList.Add(profile);
            }
            return friendList;
        }
    }
}
