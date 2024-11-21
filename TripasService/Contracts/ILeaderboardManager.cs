﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface ILeaderboardManager {
        [OperationContract]
        List<Profile> GetHighestScores();
    }
}
