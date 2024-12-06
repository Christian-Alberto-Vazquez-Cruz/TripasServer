/*using DataBaseManager.DAO;
using DataBaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TripasTests.ProxyTripas;


namespace TripasTests.DAO {

    public class LeaderboardDAOShould : IClassFixture<DBFixtureLeaderboard> {

        [Fact]
        public void GetHighestScores_ReturnsExpectedPlayers() {
            List<string> expectedUsernames = new List<string> { "Pablo", "Pinguinela", "vbox" };

            List<Perfil> highestScores = LeaderboardDAO.GetHighestScoresDAO();

            Assert.NotEmpty(highestScores);
            Assert.Equal(expectedUsernames.Count, highestScores.Count); 
            for (int i = 0; i < expectedUsernames.Count; i++) {
                Assert.Equal(expectedUsernames[i], highestScores[i].nombre);
            }
        }

        [Fact]
        public void GetHighestScores_ReturnsEmptyListWhenNoProfiles() {
            // Arrange: Elimina todos los perfiles antes de ejecutar
            using (var db = new tripasEntities()) {
                var allProfiles = db.Perfil.ToList();
                db.Perfil.RemoveRange(allProfiles);
                db.SaveChanges();
            }

            List<Perfil> highestScores = LeaderboardDAO.GetHighestScoresDAO();

            Assert.Empty(highestScores); 
        }

        [Fact]
        public void GetHighestScores_HandlesDatabaseErrorGracefully() {
            List<Perfil> highestScores = null;
            try {
                highestScores = LeaderboardDAO.GetHighestScoresDAO();
            } catch (Exception ex) {
                Assert.Contains("Error trying to retrieve the highest score players", ex.Message);
            }

            // Assert
            Assert.Null(highestScores); // Validamos que se maneje correctamente un fallo
        }
    }
}*/