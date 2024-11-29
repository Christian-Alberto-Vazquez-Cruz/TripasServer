using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TripasService.Utils;

namespace TripasService.Logic {

    [DataContract]
    public class Profile {
        [DataMember]
        public int IdProfile { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public string PicturePath { get; set; }
        [DataMember]
        public GameEnums.PlayerStatus Status { get; set; }
    }
}
