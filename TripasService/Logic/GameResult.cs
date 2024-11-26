using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class GameResult {
        [DataMember]
        public bool IsWinner { get; set; }
        [DataMember]
        public bool IsDraw { get; set; }
        [DataMember]
        public int Score { get; set; }
    }
}
