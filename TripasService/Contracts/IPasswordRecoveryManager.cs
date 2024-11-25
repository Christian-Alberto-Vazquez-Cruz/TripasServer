using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IPasswordRecoveryManager {
        [OperationContract]
        int SendRecoveryCode(string email);

        [OperationContract]
        bool VerifyRecoveryCode(string email, string code);

        [OperationContract]
        int UpdatePassword(string email, string newPassword);  
    }
}