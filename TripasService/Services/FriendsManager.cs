using DataBaseManager;
using TripasService.Logic;
using DataBaseManager.DAO;
using TripasService.Contracts;
using System.Collections.Generic;

namespace TripasService.Services {

    public partial class TripasGameService : IFriendsManager {

        public int AddFriend(int idProfile1, int idProfile2) {
            int operationResult = FriendsDAO.StrikeUpFriendshipDAO(idProfile1, idProfile2);
            return operationResult;
        }

        public int DeleteFriend(int idProfile1, int idProfile2) {
            int operationResult = FriendsDAO.DeleteFriendshipDAO(idProfile1, idProfile2);
            return operationResult;
        }

        public List<Profile> GetFriends(int idProfile) {
            List<Perfil> friendProfiles = FriendsDAO.GetFriendsDAO(idProfile);
            List<Profile> friendList = new List<Profile>();
            if (!(friendProfiles.Count == 0)) {
                foreach (Perfil friend in friendProfiles) {
                    Profile profile = new Profile() {
                        IdProfile = friend.idPerfil,
                        Username = friend.nombre,
                    };
                    friendList.Add(profile);
                }
            }
            return friendList;
        }

        public int IsFriendAlreadyAdded(int idProfile1, int idProfile2) {
            int operationResult = FriendsDAO.IsFriendAlreadyAddedDAO(idProfile1, idProfile2);
            return operationResult;
        }
    }
}
