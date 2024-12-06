using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasTests.ProxyTripas;
using System.ServiceModel;
using Xunit;

namespace TripasTests.Services {
    public class ChatManagerCallback : IChatManagerCallback {
        public bool MessageReceived { get; set; }
        public void BroadcastMessage(Message message) {
            MessageReceived = true;
        }
    }

    public class ChatManagerTest {

        private static ChatManagerClient _client;
        private static ChatManagerCallback _callback;

        public ChatManagerTest() {
            _callback = new ChatManagerCallback();
            _client = new ChatManagerClient(new InstanceContext(_callback));
            _callback.MessageReceived = false;
        }

        [Fact]
        public async void ConnectToChat() {
            _client.ConnectToChat("user1", "lobby1");
            await Task.Delay(1000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void SendMessage() {
            _client.ConnectToChat("user2", "lobby2");
            await Task.Delay(1000);
            Message mensaje = new Message() {
                Username = "user2",
                ChatMessage = "Hola desde el lobby 2"
            };
            _client.SendMessage("user2", mensaje, "lobby2");
            await Task.Delay(3000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void MultipleUsersConnectToDifferentLobbies() {
            _client.ConnectToChat("user3", "lobby3");
            await Task.Delay(1000);
            _client.ConnectToChat("user4", "lobby4");
            await Task.Delay(1000);
            Message mensaje1 = new Message() {
                Username = "user3",
                ChatMessage = "Mensaje desde lobby 3"
            };
            _client.SendMessage("user3", mensaje1, "lobby3");
            Message mensaje2 = new Message() {
                Username = "user4",
                ChatMessage = "Mensaje desde lobby 4"
            };
            _client.SendMessage("user4", mensaje2, "lobby4");
            await Task.Delay(3000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void LeaveChat() {
            _client.ConnectToChat("user5", "lobby5");
            await Task.Delay(1000);
            _client.LeaveChat("user5", "lobby5");
            await Task.Delay(1000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void BroadcastMessageToLobby() {
            _client.ConnectToChat("user6", "lobby6");
            await Task.Delay(1000);
            Message mensaje = new Message() {
                Username = "user6",
                ChatMessage = "Este es un mensaje de broadcast en lobby 6"
            };
            _client.SendMessage("user6", mensaje, "lobby6");
            await Task.Delay(3000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void ConnectToChatTwoUsersAndOneLeave() {
            _client.ConnectToChat("user7", "lobby7");
            await Task.Delay(1000);
            _client.ConnectToChat("user8", "lobby7");
            await Task.Delay(1000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void LeaveChatUser() {
            _client.ConnectToChat("user9", "lobby8");
            await Task.Delay(1000);
            _client.ConnectToChat("user10", "lobby8");
            await Task.Delay(1000);
            _client.LeaveChat("user10", "lobby8");
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void ConnectToChatTwoUsers() {
            _client.ConnectToChat("user11", "lobby9");
            await Task.Delay(1000);
            _client.ConnectToChat("user12", "lobby9");
            await Task.Delay(1000);
            Assert.True(_callback.MessageReceived);
        }

        [Fact]
        public async void ReconnectToChatTwoUsers() {
            _client.ConnectToChat("user13", "lobby10");
            await Task.Delay(1000);
            _client.ConnectToChat("user14", "lobby10");
            await Task.Delay(1000);
            _client.LeaveChat("user13", "lobby10");
            _client.ConnectToChat("user11", "lobby9");
            await Task.Delay(1000);
            Assert.True(_callback.MessageReceived);
        }

    }
}