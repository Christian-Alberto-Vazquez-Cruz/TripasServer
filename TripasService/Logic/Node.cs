using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class Node {
        [DataMember]
        public int Id { get; set; } 

        [DataMember]
        public double X { get; set; } 

        [DataMember]
        public double Y { get; set; } 

        public Node(int id, double x, double y) {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
