using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class Point {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        public Point(double x, double y) {
            X = x;
            Y = y;
        }
    }
}
