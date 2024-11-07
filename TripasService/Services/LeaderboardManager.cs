using DataBaseManager;
using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;

namespace TripasService.Services {
    partial class TripasGameService : ILeaderboardManager {
        public List<Profile> GetHighestScores() {
            List<Perfil> highestScoreProfiles = UserDAO.GetHighestScoresDAO();
            List<Profile> highestScoresList = new List<Profile>();

            foreach (var profileData in highestScoreProfiles) {
                Profile profile = new Profile() {
                    idProfile = profileData.idPerfil,
                    userName = profileData.nombre,
                    score = profileData.puntaje,
                    picturePath = profileData.fotoRuta //¿Are we going to show the profile pic? 
                };
                highestScoresList.Add(profile);
            }
            return highestScoresList;
        }

    }
}
