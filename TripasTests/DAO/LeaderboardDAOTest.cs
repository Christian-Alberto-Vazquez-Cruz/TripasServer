using DataBaseManager.DAO;
using DataBaseManager;
using TripasTests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TripasTests.ProxyTripas;


namespace TripasTests.DAO {

    public class LeaderboardDAOTest {
        [Fact]
        public void GetHighestScoresDAO() {

            int expetedResult = 10;
            List<Perfil> result = LeaderboardDAO.GetHighestScoresDAO();

            Assert.Equal(expetedResult, result.Count());
        }
    }
}