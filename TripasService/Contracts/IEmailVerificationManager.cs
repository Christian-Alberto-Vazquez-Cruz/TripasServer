using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IEmailVerificationManager {
        [OperationContract]
        [FaultContract(typeof(MailVerificationFault))]
        int sendVerificationCode(string email);

        [OperationContract]
        [FaultContract(typeof(MailVerificationFault))]
        bool verifyCode(string email, string code);

    }

    [DataContract]
    public class MailVerificationFault {
        [DataMember]
        public string ErrorMessage { get; set; }

        public MailVerificationFault(string errorMessage) {
            this.ErrorMessage = errorMessage;
        }

        public MailVerificationFault() { }
    }

}
