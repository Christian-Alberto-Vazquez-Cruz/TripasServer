using TripasService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using TripasService.Contracts.TripasService.Contracts;

namespace TripasService.Services {
    public partial class TripasGameService : IChatManager {

        private const int maxMessages = 50;
        private Queue<Message> messageHistory = new Queue<Message>();
        private Dictionary<string, IChatManagerCallBack> connectedUsers = new Dictionary<string, IChatManagerCallBack>();

        public void connectToLobby(string userName) {
            IChatManagerCallBack callback = OperationContext.Current.GetCallbackChannel<IChatManagerCallBack>();

            if (!connectedUsers.ContainsKey(userName)) {
                connectedUsers.Add(userName, callback);
                broadcastMessage(new Message($"{userName} se ha unido al lobby.", DateTime.Now, userName));
            }
            else {
                Console.WriteLine($"User {userName} is already connected.");
            }
        }

        public void leaveLobby(string userName) {
            if (connectedUsers.ContainsKey(userName)) {
                connectedUsers.Remove(userName);
                broadcastMessage(new Message($"User {userName} left the lobby.", DateTime.Now, userName));
            }
        }

        public void sendMessage(string userName, Message message) {
            if (connectedUsers.ContainsKey(userName)) {
                AddMessage(message);
                broadcastMessage(message);
            }
            else {
                Console.WriteLine($"User {userName} is not connected.");
            }
        }

        public List<Message> getMessageHistory() {
            return messageHistory.ToList();
        }

        private void AddMessage(Message message) {
            if (messageHistory.Count >= maxMessages) {
                messageHistory.Dequeue();
            }
            messageHistory.Enqueue(message);
        }

        private void broadcastMessage(Message message) {
            foreach (var user in connectedUsers.Values) {
                try {
                    user.broadcastMessage(message);
                }
                catch (Exception exception) {
                    string disconnectedUser = connectedUsers.FirstOrDefault(x => x.Value == user).Key;
                    if (disconnectedUser != null) {
                        connectedUsers.Remove(disconnectedUser);
                    }
                }
            }
        }
    }
}