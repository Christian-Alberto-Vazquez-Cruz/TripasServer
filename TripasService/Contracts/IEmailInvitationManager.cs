using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IEmailInvitationManager {

        [OperationContract]
        int SendInvitation(string username, string code);
    }
}