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
        int sendVerificationCodeRegister(string email);

        [OperationContract]
        bool verifyCode(string email, string code);
    }
}
