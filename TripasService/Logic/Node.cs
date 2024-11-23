using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TripasService.Utils;
using static TripasService.Utils.GameEnums;

namespace TripasService.Logic {
    [DataContract]
    public class Node {

        [DataMember]
        public string Id { get; set; }

 
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public GameEnums.NodeStatus Status { get; set; }

        [DataMember]
        public string Color { get; set; }

        public Node(string id, double x, double y, GameEnums.NodeStatus status = GameEnums.NodeStatus.Free) {
            Id = id;
            X = x;
            Y = y;
            Status = status;
        }

        public Node() { 

        } 
    }
}

