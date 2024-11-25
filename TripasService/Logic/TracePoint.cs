using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class TracePoint {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }
        public TracePoint(double x, double y) {
            X = x;
            Y = y;
        }
    }
}
