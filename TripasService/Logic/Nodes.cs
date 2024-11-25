using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TripasService.Utils;
using static TripasService.Utils.GameEnums;

namespace TripasService.Logic {
    [DataContract]
    public class Nodes {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public NodeStatus Status { get; set; }

        [DataMember]
        public Profile Owner { get; set; }

        public Nodes(string id, double x, double y, NodeStatus status = NodeStatus.Free) {
            Id = id;
            X = x;
            Y = y;
            Status = status;
        }

        public Nodes() {
            Id = Guid.NewGuid().ToString();
            Status = NodeStatus.Free;
        }

        public void ChangeOwnership(Profile newOwner) {
            Owner = newOwner;
            Status = newOwner != null ? NodeStatus.Occupied : NodeStatus.Free;
        }

        public bool IsConnectedTo(Nodes otherNode) {
            double distance = Math.Sqrt(Math.Pow(X - otherNode.X, 2) + Math.Pow(Y - otherNode.Y, 2));
            return distance <= 100;
        }

        public double DistanceTo(Nodes otherNode) {
            return Math.Sqrt(Math.Pow(X - otherNode.X, 2) + Math.Pow(Y - otherNode.Y, 2));
        }

        public override bool Equals(object obj) {
            if (obj is Nodes otherNode) {
                return Id == otherNode.Id;
            }
            return false;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override string ToString() {
            return $"Node {Id} at ({X}, {Y}) - Status: {Status}";
        }
    }
}
