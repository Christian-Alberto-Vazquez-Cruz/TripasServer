using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Enums {
    public static class GaneEnums {
        public enum PlayerStatus {
            Offline,
            Online,
            InGame
        }

        public enum LobbyStatus {
            Open,
            Closed,
            InProgress
        }
    }
}
