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
        int SendVerificationCodeRegister(string email);

        [OperationContract]
        bool VerifyCode(string email, string code);
    }
}
