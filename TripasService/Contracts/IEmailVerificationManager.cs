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

        /// <summary>
        /// Sends an email with a a needed verification code to register 
        /// </summary>
        /// <param name="email">Given email to register</param>
        /// <returns>Returns 1 in success and -1 if it failed</returns>
        [OperationContract]
        int SendVerificationCodeRegister(string email);

        /// <summary>
        /// Sends an email with a a needed verification code to register that expires in 60s
        /// </summary>
        /// <param name="email">Given email to register</param>
        /// <param name="verificationCode">verification code that was earlier sent to player's email</param>
        /// <returns>Returns true if code matches and false if it doesn't</returns>
        [OperationContract]
        bool VerifyCode(string email, string verificationCode);
    }
}
