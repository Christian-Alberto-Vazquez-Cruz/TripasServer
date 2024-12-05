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
                    IdProfile = profileData.idPerfil,
                    Username = profileData.nombre,
                    Score = profileData.puntaje,
                };
                highestScoresList.Add(profile);
            }
            return highestScoresList;
        }

    }
}