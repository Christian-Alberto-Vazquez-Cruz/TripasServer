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

        /// <summary>
        /// Creates a new user account with the provided login and profile information.
        /// </summary>
        /// <param name="newUser">The login information of the new user.</param>
        /// <param name="newProfile">The profile information of the new user.</param>
        /// <returns>Returns a status code indicating the result of the operation.</returns>
        [OperationContract]
        int CreateAccount(LoginUser newUser, Profile newProfile);

        /// <summary>
        /// Updates the profile information of an existing user.
        /// </summary>
        /// <param name="idProfile">The ID of the profile to be updated.</param>
        /// <param name="newUsername">The new username for the profile.</param>
        /// <param name="newPicPath">The new picture path for the profile.</param>
        /// <returns>Returns a status code indicating the result of the operation.</returns>
        [OperationContract]
        int UpdateProfile(int idProfile, string newUsername, string newPicPath);

        /// <summary>
        /// Verifies the login credentials of a user.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>Returns a status code indicating the result of the operation.</returns>
        [OperationContract]
        int VerifyLogin(string email, string password);

        /// <summary>
        /// Retrieves the profile ID associated with a given username.
        /// </summary>
        /// <param name="username">The username whose profile ID is being requested.</param>
        /// <returns>The profile ID of the user.</returns>
        /// <exception cref="ProfileNotFoundFault">Throws a fault if the profile is not found.</exception>
        [OperationContract]
        [FaultContract(typeof(ProfileNotFoundFault))]
        int GetProfileId(string username);

        /// <summary>
        /// Checks if an email address is already registered in the system.
        /// </summary>
        /// <param name="email">The email address to check for registration.</param>
        /// <returns>Returns a status code indicating if the email is registered.</returns>
        [OperationContract]
        int IsEmailRegistered (string email);

        /// <summary>
        /// Checks if a username is already registered in the system.
        /// </summary>
        /// <param name="username">The username to check for registration.</param>
        /// <returns>Returns a status code indicating if the username is registered.</returns>
        [OperationContract]
        int IsNameRegistered(string username);

        /// <summary>
        /// Retrieves the profile information of a user by their email address.
        /// </summary>
        /// <param name="email">The email of the user whose profile is being requested.</param>
        /// <returns>The profile associated with the provided email.</returns>
        [OperationContract]
        Profile GetProfileByMail(string email);

        /// <summary>
        /// Retrieves the picture path of a user by their username.
        /// </summary>
        /// <param name="username">The username of the user whose picture path is being requested.</param>
        /// <returns>The picture path of the user.</returns>
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
    [DataContract]
    public class ServiceNotAvailable {
        [DataMember]
        public string ErrorMessage { get; set; }

        public ServiceNotAvailable(string errorMessage) {
            ErrorMessage = errorMessage;
        }

        public ServiceNotAvailable() {

        }
    }
}
