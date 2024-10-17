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
        //No es oneway porque el cliente podría requerir un acuse de recibido o error en su transmisión
        [OperationContract]
        void sendMessage(Message message);

        [OperationContract]
        void connectToLobby(int roomId, string username);

        [OperationContract]
        void leaveLobby(string roomId, string username);

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
    }

}
