using TripasTests.ProxyTripas;
using System.ServiceModel;
using Xunit;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TripasService.Tests {

    public class LobbyManagerCallback : ILobbyManagerCallback {
        public bool Kicked { get; set; }
        public bool HostCallbackLeft { get; set; }
        public bool GuestCallbackLeft { get; set; }
        public bool GuestCallbackJoined { get; set; }

        public bool GameStartedNow { get; set; }

        public void KickedFromLobby() {
            Kicked = true;
        }

        public void HostLeftCallback() {
            HostCallbackLeft = true;
        }

        public void GuestLeftCallback() {
            GuestCallbackLeft = true;
        }

        public void GuestJoinedCallback(string guestName, string picturePath, int idProfile) {
            GuestCallbackJoined = true;
        }

        public void GameStarted() {
            GameStartedNow = true;
        }
        public class LobbyManagerTest {
            public static LobbyManagerClient _clientManager;
            public static LobbyManagerCallback _clientCallback;

            public LobbyManagerTest() {
                _clientCallback = new LobbyManagerCallback();
                _clientManager = new LobbyManagerClient(new InstanceContext(_clientCallback));
                _clientCallback.Kicked = false;
                _clientCallback.HostCallbackLeft = false;
                _clientCallback.GuestCallbackLeft = false;
                _clientCallback.GuestCallbackJoined = false;
                _clientCallback.GameStartedNow = false;
            }

            [Fact]
            public async void ConnectPlayerToLobby() {
                LobbyBrowserClient lobbyBrowser = new LobbyBrowserClient();
                Profile host = new Profile() {
                    IdProfile = 1,
                    Username = "Erick1"
                };

                string lobbycode = lobbyBrowser.CreateLobby("game1", 8, host);
                await Task.Delay(1000);
                Profile guest = new Profile() {
                    IdProfile = 2,
                    Username = "Christian1",
                    PicturePath = "/Images/Profiles/ImageProfile1.png"
                };

                lobbyBrowser.JoinLobby(lobbycode, guest);   
            
                _clientManager.ConnectPlayerToLobby(lobbycode, host.IdProfile);
                _clientManager.ConnectPlayerToLobby(lobbycode, guest.IdProfile);
                await Task.Delay(10000);
                Assert.True(_clientCallback.GuestCallbackJoined);
            }
        }
    }
}