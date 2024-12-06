using Moq;
using System;
using System.Collections.Generic;
using TripasTests.ProxyTripas;
using System.ServiceModel;
using Xunit;

namespace TripasService.Tests {
    public class LobbyManagerTest {
        private readonly LobbyBrowserClient _service;
        private readonly LobbyManagerClient _client;
        private readonly Mock<ILobbyManagerCallback> _mockCallback;
        private readonly Profile _hostProfile;
        private readonly Profile _guestProfile;
        private readonly Profile _hostProfile2;
        private readonly Profile _guestProfile2;

        public LobbyManagerTest() {
            _mockCallback = new Mock<ILobbyManagerCallback>();
            _service = new LobbyBrowserClient();
            _client = new LobbyManagerClient(new InstanceContext(_mockCallback.Object));
            _hostProfile = new Profile { Username = "HostPlayer", IdProfile = 1 };
            _guestProfile = new Profile { Username = "GuestPlayer", IdProfile = 2 };
            _hostProfile2 = new Profile { Username = "HostPlayer", IdProfile = 3 };
            _guestProfile2 = new Profile { Username = "GuestPlayer", IdProfile = 4 };
        }

        private string CreateMockLobby() {
            string lobbyCode = _service.CreateLobby("GameName", 10, _hostProfile);
            return lobbyCode;
        }

        [Fact]
        public void ConnectPlayerToLobbyShouldConnectHostPlayerSuccessfully() {
            string code = CreateMockLobby();
            var playerId = _hostProfile.IdProfile;
            bool result = _client.ConnectPlayerToLobby(code, playerId);
            Assert.True(result);
        }

        [Fact]
        public void ConnectPlayerToLobbyShouldFailWhenLobbyCodeIsInvalid() {
            string invalidCode = "INVALID_CODE";
            var playerId = _guestProfile.IdProfile;
            bool result = _client.ConnectPlayerToLobby(invalidCode, playerId);
            Assert.False(result);
        }

        [Fact]
        public void ConnectTwoPlayerToLobby() {
            string code = CreateMockLobby();
            var playerId = _hostProfile2.IdProfile;
            var playerId2 = _guestProfile2.IdProfile;
            _client.ConnectPlayerToLobby(code, playerId);
            bool result = _client.ConnectPlayerToLobby(code, playerId2);
            Assert.False(result);
        }

        [Fact]
        public void ConnectTwoPlayerToLobbyShouldFailWhenLobbyCodeIsInvalid() {
            string code = CreateMockLobby();
            string invalidCode = "INVALID_CODE";
            var playerId = _hostProfile2.IdProfile;
            var playerId2 = _guestProfile2.IdProfile;
            _client.ConnectPlayerToLobby(code, playerId);
            bool result = _client.ConnectPlayerToLobby(invalidCode, playerId2);
            Assert.False(result);
        }
    }
}