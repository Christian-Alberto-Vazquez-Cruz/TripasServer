using TripasService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;

namespace TripasService.Services {

    public partial class TripasGameService : IChatManager {

        private Dictionary<string, IChatManagerCallBack> connectedUsers = new Dictionary<string, IChatManagerCallBack>();
        // ((ICommunicationObject)callback).Close();
        public void ConnectToChat(string userName) {
            IChatManagerCallBack callback = OperationContext.Current.GetCallbackChannel<IChatManagerCallBack>();

            if (!connectedUsers.ContainsKey(userName)) {
                connectedUsers.Add(userName, callback);
                Console.WriteLine($"{userName} se ha conectado al chat.");
                BroadcastMessage(new Message($"{userName} se ha unido al lobby.", DateTime.Now, userName));
            }
            else {
                Console.WriteLine($"User {userName} is already connected.");
            }
        }
        
        public void LeaveChat(string userName) {
            if (connectedUsers.ContainsKey(userName)) {
                connectedUsers.Remove(userName);
                BroadcastMessage(new Message($"User {userName} left the lobby.", DateTime.Now, userName));
            }
        }

        public void SendMessage(string userName, Message message) {
            if (connectedUsers.ContainsKey(userName)) {
                BroadcastMessage(message);
            }
            else {
                Console.WriteLine($"User {userName} is not connected.");
            }
        }

        private void BroadcastMessage(Message message) {
            Console.WriteLine($"Se está haciendo el broadcast con {message}");
            foreach (var user in connectedUsers.Keys)
            {
                Console.WriteLine($"{user}");
            }
            foreach (var user in connectedUsers.Values) {
                try {
                    user.BroadcastMessage(message);
                }
                catch (Exception exception) {
                    /*string disconnectedUser = connectedUsers.FirstOrDefault(x => x.Value == user).Key;
                    if (disconnectedUser != null) {
                        connectedUsers.Remove(disconnectedUser);
                    }*/
                    Console.WriteLine($"Ha ocurrido una excepción con el BroadCast {exception}");
                }
            }
        }
    }
}