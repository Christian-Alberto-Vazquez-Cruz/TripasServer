using TripasService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;
using System.Collections.Concurrent;
namespace TripasService.Services {
    public partial class TripasGameService : IChatManager {
        private IChatManagerCallBack callback;
        private ConcurrentDictionary<string, IChatManagerCallBack> connectedUsers = new ConcurrentDictionary<string, IChatManagerCallBack>();

        public void ConnectToChat(string userName) {
            callback = OperationContext.Current.GetCallbackChannel<IChatManagerCallBack>();

            if (connectedUsers.TryAdd(userName, callback)) {
                Console.WriteLine($"{userName} se ha conectado al chat (se ejecutó ConnectToChat)");
                BroadcastMessage(new Message($"{userName} se ha unido al lobby.", DateTime.Now, userName));
            } else {
                Console.WriteLine($"Usuario {userName} ya se encuentra conectado.");
            }
        } 

        public void LeaveChat(string userName) {
            if (connectedUsers.TryRemove(userName, out _)) {
                BroadcastMessage(new Message($"User {userName} abandonó el lobby.", DateTime.Now, userName));
            }
        }

        public void SendMessage(string userName, Message message) {
            if (connectedUsers.ContainsKey(userName)) {
                BroadcastMessage(message); 
            } else {
                Console.WriteLine($"Usuario {userName} no está conectado.");
            }
        }

        private void BroadcastMessage(Message message) {
            Console.WriteLine($"Se está haciendo el broadcast con {message}");
            foreach (var user in connectedUsers.Values) {
                try {
                    user.BroadcastMessage(message);
                } catch (Exception exception) {
                    string disconnectedUser = connectedUsers.FirstOrDefault(x => x.Value == user).Key;
                    if (disconnectedUser != null) {
                        connectedUsers.TryRemove(disconnectedUser, out _);
                        Console.WriteLine($"Ha ocurrido una excepción con el BroadCast {exception}");
                    }
                }
            }
        }
    }
}


