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
        public int idProfile { get; set; }
        [DataMember]
        public string userName { get; set; }
        [DataMember]
        public int score { get; set; }
        [DataMember]
        public string picturePath { get; set; }
        [DataMember]
        public GameEnums.PlayerStatus status { get; set; }
    }
}
