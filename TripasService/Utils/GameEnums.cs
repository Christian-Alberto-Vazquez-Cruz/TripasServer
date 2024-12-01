using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Utils {
    public static class GameEnums {
        public enum PlayerStatus {
            Offline,
            Online,
        }

        public enum NodeStatus {
            Free,
            Occupied,
        }
    }
}
