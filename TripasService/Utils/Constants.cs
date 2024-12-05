using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Utils {
    public static class Constants {
        public const int SUCCESSFUL_OPERATION = 1;
        public const int FAILED_OPERATION = -1;
        public const int NO_MATCHES = -2; 
        public const int FOUND_MATCH = 2;
        public const string NO_MATCHES_STRING = "No matches found";
        public const string FAILED_OPERATION_STRING = "Operation failed";
        public const int MIN_ID_GUEST = 100000;
        public const int MAX_ID_GUEST = 110001;
        public const int EMPTY_STRUCTURE = -3;

        public const int INITIAL_SCORE = 0;
        public const string DEFAULT_PICPATH = "/Images/Profiles/ImageProfile1.png";

        public const int MIN_TRACE_SCORE = 20;
        public const int MAX_TRACE_SCORE = 100;
        public const int MIN_POINTS_CRITERIA = 1;
        public const int MAX_POINTS_CRITERIA = 800;
        public const int MAX_MIN_TRACE_SCORE_DIFF = MAX_TRACE_SCORE - MIN_TRACE_SCORE;
        public const int MIN_MAX_POINTS_CRITERIA_DIFF = MAX_POINTS_CRITERIA - MIN_POINTS_CRITERIA;
    }
}
