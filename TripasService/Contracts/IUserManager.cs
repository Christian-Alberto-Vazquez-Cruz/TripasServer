using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Utils;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IUserManager {
        [OperationContract]
        int CreateAccount(LoginUser user, Profile profile);

        [OperationContract]
        int UpdateProfile(int idProfile, string newUsername, string newPic);

        [OperationContract]
        int VerifyLogin(string mail, string password);

        [OperationContract]
        [FaultContract(typeof(ProfileNotFoundFault))]
        int GetProfileId(string userName);

        [OperationContract]
        bool IsEmailRegistered (string mail);

        [OperationContract]
        int UpdateProfileName(int idProfile, string newProfileName);

        [OperationContract]
        int UpdateProfilePic(int idProfile, string newProfilePic);

        [OperationContract]
        Profile GetProfileByMail(string mail);

    }

    [DataContract]
    public class Profile {
        [DataMember]
        public int idProfile { get; set; }
        [DataMember]
        public string userName { get; set; }
        [DataMember]
        public int score { get; set; }
        [DataMember]
        public string picturePath { get; set; }
        [DataMember]
        public GameEnums.PlayerStatus status { get; set; }


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
