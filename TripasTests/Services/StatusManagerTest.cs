using DataBaseManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasTests.ProxyTripas;
using Xunit;

namespace TripasTests.Services {
    public class StatusManagerTest {
        private readonly ProxyTripas.StatusManagerClient _statusManager;

        public StatusManagerTest() {
            _statusManager = new ProxyTripas.StatusManagerClient(); 
        }

        [Fact]
        public void GetPlayerStatusOffline() {
            int idTest = 1001;

            GameEnumsPlayerStatus resultExpected = GameEnumsPlayerStatus.Offline;
            GameEnumsPlayerStatus resultObtained = _statusManager.GetPlayerStatus(idTest);

            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void GetPlayerStatusOnline() {
            int idTest = 1002;

            _statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Online);

            GameEnumsPlayerStatus resultExpected = GameEnumsPlayerStatus.Online;
            GameEnumsPlayerStatus resultObtained = _statusManager.GetPlayerStatus(idTest);

            Assert.Equal(resultExpected, resultObtained);
        }

        [Fact]
        public void SetPlayerStatusOnline() {
            int idTest = 2000;

            bool resultObtained = _statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Online);

            Assert.True(resultObtained);
        }

        [Fact]
        public void SetPlayerStatusOffline() {
            int idTest = 2000;

            bool resultObtained = _statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Offline);

            Assert.True(resultObtained);
        }

        [Fact]
        public void DisconnectPlayer() {
            int idTest = 1003;
            _statusManager.SetPlayerStatus(idTest, GameEnumsPlayerStatus.Online);

            int resultExpected = Constants.SUCCESSFUL_OPERATION;
            int resultObtained = _statusManager.DisconnectPlayer(idTest);

            Assert.Equal(resultExpected, resultObtained);
        }


        [Fact]
        public void DisconnectPlayerNoMatches() {
            int idTest = 1003;

            int resultExpected = Constants.NO_MATCHES;
            int resultObtained = _statusManager.DisconnectPlayer(idTest);

            Assert.Equal(resultExpected, resultObtained);
        }
    }
}
