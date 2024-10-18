using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {


    namespace TripasService.Contracts {

        [ServiceContract(CallbackContract = typeof(IChatManagerCallBack))]
        public interface IChatManager {
            [OperationContract]
            void sendMessage(string userName, Message message);

            [OperationContract]
            void connectToLobby(string userName);

            [OperationContract]
            void leaveLobby(string userName);

            [OperationContract]
            List<Message> getMessageHistory();
        }

        public interface IChatManagerCallBack {
            [OperationContract(IsOneWay = true)]
            void broadcastMessage(Message message);
        }

        [DataContract]
        public class Message {
            [DataMember]
            public DateTime timeStamp { get; set; } = DateTime.Now;

            [DataMember]
            public string chatMessage { get; set; }

            [DataMember]
            public string userName { get; set; }

            public Message(string chatMessage, DateTime timeStamp, string userName) {
                this.chatMessage = chatMessage;
                this.timeStamp = timeStamp;
                this.userName = userName;
            }

            public override string ToString() {
                return $"{timeStamp.ToLocalTime()} {userName}: {chatMessage}";
            }
        }
    }
}