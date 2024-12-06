using TripasTests.ProxyTripas;
using System.ServiceModel;
using Xunit;
using System.Threading.Tasks;

namespace TripasService.Tests {

    public class LobbyManagerCallback : ILobbyManagerCallback {
        public bool kickedFromLobby { get; set; }
        public bool hostLeftCallback { get; set; }
        public bool guestLeftCallback { get; set; }
        public bool guestJoinedCallback { get; set; }

        public bool gameStarted { get; set; }

        public void KickedFromLobby() {
            kickedFromLobby = true;
        }

        public void HostLeftCallback() {
            hostLeftCallback = true;
        }

        public void GuestLeftCallback() {
            guestLeftCallback = true;
        }

        public void GuestJoinedCallback(string guestName, string picturePath, int idProfile) {
            guestJoinedCallback = true;
        }

        public void GameStarted() {
            gameStarted = true;
        }
        public class LobbyManagerTest {
            public static LobbyManagerClient _clientManager;
            public static LobbyManagerCallback _clientCallback;

            public LobbyManagerTest() {
                _clientCallback = new LobbyManagerCallback();
                _clientManager = new LobbyManagerClient(new InstanceContext(_clientCallback));
                _clientCallback.kickedFromLobby = false;
                _clientCallback.hostLeftCallback = false;
                _clientCallback.guestLeftCallback = false;
                _clientCallback.guestJoinedCallback = false;
                _clientCallback.gameStarted = false;
            }

            [Fact]
            public async void ConnectPlayerToLobby() {
                LobbyBrowserClient lobbyBrowser = new LobbyBrowserClient();
                Profile host = new Profile();
                host.IdProfile = 1;
                host.Username = "erick1";
                string lobbycode = lobbyBrowser.CreateLobby("game1", 8, host);
                await Task.Delay(1000);
                Profile guest = new Profile();
                guest.IdProfile = 2;
                guest.Username = "Chistian1";
                guest.PicturePath = "/imagen";
                _clientManager.ConnectPlayerToLobby(lobbycode, guest.IdProfile);
                await Task.Delay(10000);
                Assert.True(_clientCallback.guestJoinedCallback);
            }
        }
    }
}