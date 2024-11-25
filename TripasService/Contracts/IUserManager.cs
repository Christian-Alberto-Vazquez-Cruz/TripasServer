using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Utils;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IUserManager {
        [OperationContract]
        int CreateAccount(LoginUser user, Profile profile);

        [OperationContract]
        int UpdateProfile(int idProfile, string newUsername, string newPic);

        [OperationContract]
        int VerifyLogin(string email, string password);

        [OperationContract]
        [FaultContract(typeof(ProfileNotFoundFault))]
        int GetProfileId(string username);

        [OperationContract]
        int IsEmailRegistered (string email);

        [OperationContract]
        int IsNameRegistered(string username);

        [OperationContract]
        Profile GetProfileByMail(string email);

        [OperationContract]
        string GetPicPath(string username);

    }


    [DataContract]
    public class LoginUser {
        [DataMember]
        public int idLoginUser { get; set; }
        [DataMember]
        public string mail { get; set; }
        [DataMember]
        public string password { get; set; }
    }

    [DataContract]
    public class ProfileNotFoundFault {
        [DataMember]
        public string errorMessage { get; set; }

        public ProfileNotFoundFault(string errorMessage) {
            this.errorMessage = errorMessage;
        }

        public ProfileNotFoundFault() {
        }
    }
}
