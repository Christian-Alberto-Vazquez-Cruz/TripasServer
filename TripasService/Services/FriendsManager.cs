using DataBaseManager;
using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using TripasService.Logic;

namespace TripasService.Services {
    public partial class TripasGameService : IFriendsManager {
        public int AddFriend(int idProfile1, int idProfile2) {
            int result = FriendsDAO.StrikeUpFriendshipDAO(idProfile1, idProfile2);
            return result;
        }
        public int DeleteFriend(int idProfile1, int idProfile2) {
            int result = FriendsDAO.DeleteFriendshipDAO(idProfile1, idProfile2);
            return result;
        }

        public List<Profile> GetFriends(int idProfile) {
            List<Perfil> friendProfiles = FriendsDAO.GetFriendsDAO(idProfile);
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
    }
}
    