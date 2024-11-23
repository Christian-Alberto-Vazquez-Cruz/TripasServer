using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class Trace {
        [DataMember]
        public string Player { get; set; }

        [DataMember]
        public Node StartNode { get; set; }

        [DataMember]
        public Node EndNode { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public List<TracePoint> TracePoints { get; set; } = new List<TracePoint>();
        [DataMember]
        public string Color { get; set; }

        public Trace() {

        }
    }
}
