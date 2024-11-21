using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseManager.DAO {
    public class LeaderboardDAO {
        public static List<Perfil> GetHighestScoresDAO() {
            List<Perfil> bestPlayersList = new List<Perfil>();
            try {
                using (tripasEntities db = new tripasEntities()) {
                    var profiles = db.Perfil.OrderByDescending(perfil => perfil.puntaje).Take(Constants.HOW_MANY_SCORES).ToList();
                    bestPlayersList = profiles;
                }
            } catch (EntityException entityException) {
                Console.WriteLine($"Error trying to retrieve the highest score players {entityException.Message}");
            }
            return bestPlayersList;
        }
    }
}
