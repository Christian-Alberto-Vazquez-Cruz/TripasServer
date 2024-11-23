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
        bool RegisterTrace(string matchCode, Trace trace);

        [OperationContract]
        bool RegisterPlayerCallback(string matchCode, string username);

        [OperationContract]
        List<Node> GetNodes(string matchCode);

        [OperationContract]
        Dictionary<string, string> GetNodePairs(string matchCode);
    }

    [ServiceContract]
    public interface IMatchManagerCallback {

        [OperationContract(IsOneWay = true)]
        void MatchEnded(string matchCode);

        [OperationContract(IsOneWay = true)]
        void TraceReceived(Trace trace);
    }
}
