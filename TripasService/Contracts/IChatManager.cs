using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {

    [ServiceContract(CallbackContract = typeof(IChatManagerCallBack))]
    public interface IChatManager {

        [OperationContract(IsOneWay = true)]
        void SendMessage(string userName, Message message, string lobbyCode);

        [OperationContract(IsOneWay = true)]
        void ConnectToChat(string userName, string lobbyCode);

        [OperationContract(IsOneWay = true)]
        void LeaveChat(string userName, string lobbyCode);

    }
    [ServiceContract]
    public interface IChatManagerCallBack {
        [OperationContract]
        void BroadcastMessage(Message message);

    }

    [DataContract]
    public class Message {
        [DataMember]
        public DateTime timeStamp { get; set; } 

        [DataMember]
        public string chatMessage { get; set; }

        [DataMember]
        public string userName { get; set; }

       public Message(string chatMessage, DateTime timeStamp, string userName) {
            this.chatMessage = chatMessage;
            this.timeStamp = timeStamp;
            this.userName = userName;
        }

        public Message() {

        }
        public override string ToString() {
            return $"{timeStamp.ToLocalTime()} {userName}: {chatMessage}";
        }
    }
}
