using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Logic {
    [DataContract]
    public class Message {
        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public string ChatMessage { get; set; }

        [DataMember]
        public string Username { get; set; }

        public Message(string chatMessage) {
            this.ChatMessage = chatMessage;
            this.TimeStamp = DateTime.Now;
        }

        public Message() {

        }
        public override string ToString() {
            return $"{TimeStamp.ToLocalTime()} {Username}: {ChatMessage}";
        }
    }
}