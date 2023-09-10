using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public enum ItemAction
    {
        None = 0,
        Buy = 1,
        Sell = 2,
        Drop = 3,
        Use = 4,
        Equip = 5,
        Unequip = 6,
        SplitItem = 7,
        CombineItem = 8,
        GetWarehouseGold = 9,
        DepositGold = 10,
        WithdrawGold = 11,
        DropMoney = 12,
        SpendMoney = 13,
        Repair = 14,
        RepairAll = 15,
        Ident = 16,
        Durability = 17,
        DropEquipment = 18,
        UpQuality = 19,
        UpLevel = 20,
        BoothQuery = 21,
        BoothAdd = 22,
        BoothDelete = 23,
        BoothBuy = 24,
        SynchroAmount = 25,
        Fireworks = 26,
        Ping = 27,
        Enchant = 28,
        BoothAddEMoney = 29,
        DetainItemRedeem = 32,
        DetainItemClaim = 33,
        TalismanSocket = 35,
        TalismanSocketEMoney = 36,
        DropItem = 37,
        GemCompose = 39,
        Bless = 40,
        Activate = 41,
        CreateSocket = 43,
        Equipment = 46,
        ComposeTortoise = 51,
        QueryChatItem = 52
    }
}
