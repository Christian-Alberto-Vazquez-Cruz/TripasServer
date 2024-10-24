﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IUserManager {
        [OperationContract]
        int createAccount(LoginUser user, Profile profile);

        [OperationContract]
        int updateProfile(Profile profile);

        [OperationContract]
        Profile getProfile(String email, String password);
        [OperationContract]
        int verifyLogin(LoginUser user);

        [OperationContract]
        [FaultContract(typeof(ProfileNotFoundFault))]
        int getProfileId(string userName);
        [OperationContract]
        bool isEmailRegistered (string email);
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
