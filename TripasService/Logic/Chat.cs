using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripasService.Contracts;

namespace TripasService.Logic {
    public class Chat {
        ConcurrentDictionary<string, IChatManagerCallBack> playerInChat = new ConcurrentDictionary<string, IChatManagerCallBack>();
        



    }
}
