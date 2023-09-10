using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public enum ItemPosition
    {
        Inventory = 0,

        Set1Helmet = 1,
        Set1Necklace,
        Set1Armor,
        Set1Weapon1,
        Set1Weapon2,
        Set1Ring,
        Set1Gourd,
        Set1Boots,
        Set1Garment,

        Fan,
        Tower,
        Steed,
        W1Accessory = 15,
        W2Accessory = 16,
        SteedAccessory = 17,
        Crop = 18,

        Set2Helmet = Set1Helmet + 20,
        Set2Necklace = Set1Necklace + 20,
        Set2Armor = Set1Armor + 20,
        Set2Weapon1 = Set1Weapon1 + 20,
        Set2Weapon2 = Set1Weapon2 + 20,
        Set2Ring = Set1Ring + 20,
        Set2Boots = Set1Boots + 20,
        Set2Garment = Set1Garment + 20,

        /*MarketWarehouse = 100,
        PowerWarehouse,
        TwinCityWarehouse,
        PokerWarehouse,
        DesertCityWarehouse,
        ApeCityWarehouse,
        BirdIslandWarehouse,
        PhoenixCastleWarehouse,
        StoneCityWarehouse,*/
    }
}
