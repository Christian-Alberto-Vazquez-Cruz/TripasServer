using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;

namespace TripasService.Services {
    public partial class TripasGameService : IChatManager {
        public void connectToLobby(int roomId, string username) {
            throw new NotImplementedException();
        }

        public void leaveLobby(string roomId, string username) {
            throw new NotImplementedException();
        }

        public void sendMessage(Message message) {
            throw new NotImplementedException();
        }
    }
}
