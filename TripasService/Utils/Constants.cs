using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Utils {
    public static class Constants {
        public const int SUCCESSFUL_OPERATION = 1;
        public const int FAILED_OPERATION = -1;
        public const int NO_MATCHES = -2; 
        public const int FOUND_MATCH = 2;
        public const int MAX_MESSAGES = 50;
    }
}
