﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface IFriendsManager {
        [OperationContract]
        int addFriend(int idProfile1, int idProfile2);
        [OperationContract]
        int deleteFriend(int idProfile1, int idProfile2);
        [OperationContract]
        List<Profile> getFriends(int idProfile);
    }
}