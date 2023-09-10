using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public enum ItemInfoAction
    {
        None = 0,
        AddItem = 1,
        Trade = 2,
        Update = 3,
        OtherPlayerEquipment = 4,
        AddPackageUnbindingItem = 5, // whatever that means
        DeleteTradeItem = 7,
        AddLostDepotItem = 8,
        ChatItem = 9,
        RollItem = 10,
        Mail = 11,
        Auction = 12
    }
}
