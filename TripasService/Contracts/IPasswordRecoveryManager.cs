using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IPasswordRecoveryManager {

        /// <summary>
        /// Sends an email with a a needed verification code to change the player password that expires in 60s
        /// </summary>
        /// <param name="email">Given email to register</param>
        /// <returns>Returns 1 if code was sent, -1 if failed and -2 if the email didn't exist </returns>
        [OperationContract]
        int SendRecoveryCode(string email);

        /// <summary>
        /// Verifies the email and code against internal asocciated code
        /// </summary>
        /// <param name="email">Given email to register</param>
        /// <param name="code">Verification code that was earlier sent to player's email</param>
        /// <returns>Returns true if code matches and false if it doesn't</returns>
        [OperationContract]
        bool VerifyRecoveryCode(string email, string code);

        /// <summary>
        /// Lets the user update his/her account password for a new one
        /// </summary>
        /// <param name="email">Player's email</param>
        /// <param name="newPassword">New password that the player account will have</param>
        /// <returns>Returns 1 if the operation was successfull, -1 if it failed or -2 if email wasn't foud</returns>
        [OperationContract]
        int UpdatePassword(string email, string newPassword);  
    }
}