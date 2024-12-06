using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IEmailInvitationManager {

        /// <summary>
        /// Sends an email with the current lobby code for a friend to join
        /// </summary>
        /// <param name="username">User in friendlist to invite</param>
        /// <param name="code">Lobby code</param>
        /// <returns>Returns 1 in sucess and -1 if it failed</returns>
        [OperationContract]
        int SendInvitation(string username, string code);
    }
}