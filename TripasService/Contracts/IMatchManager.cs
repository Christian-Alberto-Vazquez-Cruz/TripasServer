using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager {
        [OperationContract]
        bool CreateMatch(string code, string gameName, int nodeCount, Dictionary<string, Profile> players);

        [OperationContract]
        bool ConnectNodes(string matchCode, int node1, int node2);

        [OperationContract]
        Match GetMatchStatus(string matchCode);
    }

    public interface IMatchManagerCallback {
        [OperationContract(IsOneWay = true)]
        void NodeConnected(int node1, int node2);

        [OperationContract(IsOneWay = true)]
        void MatchEnded(string matchCode);
    }
}
