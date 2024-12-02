using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class LoginUser {
        [DataMember]
        public int IdLoginUser { get; set; }
        [DataMember]
        public string Mail { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}