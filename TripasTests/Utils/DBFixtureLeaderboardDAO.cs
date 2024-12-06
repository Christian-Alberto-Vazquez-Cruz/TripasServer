/*using DataBaseManager.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasTests.Utils {
    public class DBFixtureLeaderboard : IDisposable {
        private readonly List<int> _addedLeaderboardEntries = new List<int>();

        public DBFixtureLeaderboard() {
            // Agregar entradas al leaderboard para las pruebas de LeaderboardDAO
            AddLeaderboardEntry(1, 100);  // ID de usuario 1, con 100 puntos
            AddLeaderboardEntry(2, 200);  // ID de usuario 2, con 200 puntos
        }

        private void AddLeaderboardEntry(int userId, int score) {
            // Imagina que aquí insertamos los datos del leaderboard en la base de datos
            LeaderboardDAO.AddLeaderboardEntry(userId, score);
            _addedLeaderboardEntries.Add(userId);
        }

        public void Dispose() {
            // Limpiar los datos después de cada prueba
            foreach (int userId in _addedLeaderboardEntries) {
                LeaderboardDAO.DeleteLeaderboardEntry(userId);
            }
        }
    }
}*/