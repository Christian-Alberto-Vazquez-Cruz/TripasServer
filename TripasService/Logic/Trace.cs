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
        public List<TracePoint> TracePoints { get; set; } = new List<TracePoint>();

        [DataMember]
        public string Color { get; set; }

        /* This calculus implies that every trace will at least be worth of MIN_POINTS_CRITERIA no matter it's size 
         * it will increase until MAX_POINTS_CRITERIA is reached, progressively
         */
        public int Score {
            get {
                int pointsCount = TracePoints.Count;

                if (pointsCount < Constants.MIN_POINTS_CRITERIA) {
                    return Constants.MIN_TRACE_SCORE;
                } else if (pointsCount > Constants.MAX_POINTS_CRITERIA) {
                    return Constants.MAX_TRACE_SCORE;
                } else {
                    return Constants.MIN_TRACE_SCORE +
                           ((pointsCount - Constants.MIN_POINTS_CRITERIA) * Constants.MAX_MIN_TRACE_SCORE_DIFF) /
                           Constants.MIN_MAX_POINTS_CRITERIA_DIFF;
                }
            }
        }
        public Trace() {

        }
    }
}
