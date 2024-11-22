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
        public string Player { get; set; } //Identifica al jugador que hace el trazo

        [DataMember]
        public Node StartNode { get; set; }

        [DataMember]
        public Node EndNode { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        // Coordenadas del trazo para dibujo
        [DataMember]
        public List<TracePoint> TracePoints { get; set; } = new List<TracePoint>();

        // Color del trazo
        [DataMember]
        public string Color { get; set; }
        
        public Trace() {

        }
    }
}
