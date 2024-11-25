using System;
using System.Collections.Generic;
using System.Linq;
using TripasService.Utils;
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
        public int Score {
            get {
                int pointsCount = TracePoints.Count;

                return pointsCount < Constants.MIN_POINTS_CRITERIA 
                       ? Constants.MIN_TRACE_SCORE
                       : TracePoints.Count > Constants.MAX_POINTS_CRITERIA 
                       ? Constants.MAX_TRACE_SCORE
                       : Constants.MIN_TRACE_SCORE + ((TracePoints.Count - Constants.MIN_POINTS_CRITERIA) * 
                       Constants.MAX_MIN_TRACE_SCORE_DIFF) / Constants.MIN_MAX_POINTS_CRITERIA_DIFF;
            }
        }


        public Trace() {

        }
    }
}
