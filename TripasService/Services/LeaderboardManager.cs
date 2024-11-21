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
    partial class TripasGameService : ILeaderboardManager {
        public List<Profile> GetHighestScores() {
            List<Perfil> highestScoreProfiles = LeaderboardDAO.GetHighestScoresDAO();
            List<Profile> highestScoresList = new List<Profile>();

            foreach (var profileData in highestScoreProfiles) {
                Profile profile = new Profile() {
                    idProfile = profileData.idPerfil,
                    userName = profileData.nombre,
                    score = profileData.puntaje,
                    picturePath = profileData.fotoRuta //¿Se mostrará la foto?
                };
                highestScoresList.Add(profile);
            }
            return highestScoresList;
        }

    }
}
