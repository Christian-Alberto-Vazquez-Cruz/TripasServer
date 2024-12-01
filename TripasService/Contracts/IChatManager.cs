using System;
using System.Collections.Generic;
using System.Linq;
using TripasService.Logic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {

    [ServiceContract(CallbackContract = typeof(IChatManagerCallBack))]
    public interface IChatManager {

        [OperationContract(IsOneWay = true)]
        void SendMessage(string username, Message message, string lobbyCode);

        [OperationContract(IsOneWay = true)]
        void ConnectToChat(string username, string lobbyCode);

        [OperationContract(IsOneWay = true)]
        void LeaveChat(string username, string lobbyCode);

    }
    [ServiceContract]
    public interface IChatManagerCallBack {
        [OperationContract]
        void BroadcastMessage(Message message);

    }
}
