using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;
using TripasService.Utils;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IUserManager {
        [OperationContract]
        int CreateAccount(LoginUser newUser, Profile newProfile);

        [OperationContract]
        int UpdateProfile(int idProfile, string newUsername, string newPicPath);

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
    public class ProfileNotFoundFault {
        [DataMember]
        public string ErrorMessage { get; set; }

        public ProfileNotFoundFault(string errorMessage) {
            this.ErrorMessage = errorMessage;
        }

        public ProfileNotFoundFault() {
        }
    }
}
