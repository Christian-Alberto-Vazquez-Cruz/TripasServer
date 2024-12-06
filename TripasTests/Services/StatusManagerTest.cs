using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasTests.ProxyTripas;
using Xunit;

namespace TripasTests.Services {
    public class StatusManagerTest {
        [Fact]
        public void GetPlayerStatusOffline() {
            ProxyTripas.StatusManagerClient statusManager = new ProxyTripas.StatusManagerClient();
            int idTest = 1001;

            GameEnumsPlayerStatus resultExpected = GameEnumsPlayerStatus.Offline;
            GameEnumsPlayerStatus resultObtained = statusManager.GetPlayerStatus(idTest);

            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void GetPlayerStatusOnline() {
            ProxyTripas.StatusManagerClient statusManager = new ProxyTripas.StatusManagerClient();
            int idTest = 1002;

            statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Online);

            GameEnumsPlayerStatus resultExpected = GameEnumsPlayerStatus.Online;
            GameEnumsPlayerStatus resultObtained = statusManager.GetPlayerStatus(idTest);

            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void SetPlayerStatusOnline() {
            ProxyTripas.StatusManagerClient statusManager = new ProxyTripas.StatusManagerClient();
            int idTest = 2000;

            bool resultObtained = statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Online);

            Assert.True(resultObtained);
        }

        [Fact]
        public void SetPlayerStatusOffline() {
            ProxyTripas.StatusManagerClient statusManager = new ProxyTripas.StatusManagerClient();
            int idTest = 2000;

            bool resultObtained = statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Offline);

            Assert.True(resultObtained);
        }
    }
}
