﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TripasService.Logic;

namespace TripasService.Contracts {
    [ServiceContract]
    public interface ILobbyBrowser {
        [OperationContract]
        List<Lobby> GetAvailableLobbies();

        [OperationContract]
        bool JoinLobby(string code, Profile guest);

        [OperationContract]
        string CreateLobby(string gameName, int nodeCount, Profile owner);

        [OperationContract]
        Lobby GetLobbyByCode(string code);
    }
}