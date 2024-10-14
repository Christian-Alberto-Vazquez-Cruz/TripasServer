using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;


namespace TripasService.Services {
    [ServiceBehavior]
    public partial class TripasGameService : IUserManager {
        public int createAccount(LoginUser user, Profile profile) {
            return 1;
        }

        public Profile getProfile(string email) {
            throw new NotImplementedException();
        }

        public int updateAccount(Profile profile) {
            throw new NotImplementedException();
        }
    }
}
