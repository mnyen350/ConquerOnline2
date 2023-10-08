using ConquerServer.Database;
using ConquerServer.Database.Models;
using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public enum ItemSort
    {
        Invalid = -1,
        Expendable = 0,
        Helmet = 1,
        Necklace = 2,
        Armor = 3,
        Weapon1 = 4,
        Weapon2 = 5,
        Shield = 6,
        RingR = 7,
        Shoes = 8,
        Other = 9,
        RingL = 10,
        Overcoat = 11,
        DamageArtifact = 12,
        Unknown13 = 13,
        Mount = 14,
        Weapon1Coat = 15,
        Weapon2Coat = 16,
        BowCoat = 17,
        ShieldCoat = 18,
        MountDecorator = 20,
        HorseWhip = 21
    }

    public enum ItemType
    {
        Invalid = -1,

        // DamageArtifact
        IncreaseDmgArtifact = 1,

        DecreaseDmgArtifact = 2,

        // Weapon1
        Blade = 10000,

        Sword = 20000,
        Backsword = 21000,
        Hook = 30000,
        Whip = 40000,
        Axe = 50000,
        Hammer = 60000,
        Club = 80000,
        Scepter = 81000,
        Dagger = 90000,

        // Weapon1 (two)
        NinjaSword = 01000,

        PrayerBeads = 10000,
        Rapier = 11000,
        Pistol = 12000,
        AssassinKnife = 13000,

        // Weapon2

        Glaive = 10000,
        Scythe = 11000,
        Poleaxe = 30000,
        Longhammer = 40000,
        Spear = 60000,
        Wand = 61000,
        Pickaxe = 62000,
        Halbert = 80000,

        // Other
        Gem = 00000,

        TaskItem = 10000,
        ActionItem = 20000,
        ComposeItem = 30000,
        MonsterItem = 50000,
        PointCard = 80000,
        DarkHorn = 90000,

        // Expendable
        Physic = 00000,

        PhysicMana = 01000,
        PhysicLife = 02000,
        Spell = 60000,
        Ore = 70000,
        Special = 80000,
        Silver = 90000,

        // MountAndAccessory
        Mount = 00000,

        Weapon2Coat = 50000,
        Weapon1Coat = 60000,
        BowCoat = 70000,
        ShieldCoat = 80000,

        /// -----


        // 2h
        Bow = 323,

        // Expendable
        Arrow = 50000,
    }

    public class Item
    {
        public static ItemType GetSubType(int typeId)
        {
            switch (GetSort(typeId))
            {
                case ItemSort.Weapon1:
                case ItemSort.Weapon2:
                    return (ItemType)(typeId % 100000 / 1000 * 1000);

                case ItemSort.Other:
                case ItemSort.RingL:
                    return (ItemType)(typeId % 100000 / 10000 * 10000);

                case ItemSort.DamageArtifact:
                    return (ItemType)(typeId % 10000 / 1000);

                case ItemSort.Expendable:
                    switch (typeId / 1000)
                    {
                        case 1050:
                            return ItemType.Arrow;

                        case 1000:
                        case 1001:
                        case 1002:
                            return ItemType.Physic;

                        case 1072:
                            return ItemType.Ore;
                    }
                    break;
            }

            return ItemType.Invalid;
        }

        public static ItemSort GetSort(int typeId)
        {
            switch (typeId % 10000000 / 100000)
            {
                case 1:
                    {
                        switch (typeId % 1000000 / 10000)
                        {
                            case 11: return ItemSort.Helmet;
                            case 12: return typeId / 1000 == 123 ? ItemSort.Helmet : ItemSort.Necklace;
                            case 13: return ItemSort.Armor;
                            case 14: return ItemSort.Helmet;
                            case 15: return ItemSort.RingR;
                            case 16: return ItemSort.Shoes;
                            case 17: break;
                            case 18: return ItemSort.Overcoat;
                            case 19: return ItemSort.Overcoat;
                        }
                        break;
                    }
                case 2:
                    {
                        switch (typeId % 10000 / 1000)
                        {
                            case 0: return ItemSort.MountDecorator;
                            case 1: return ItemSort.DamageArtifact;
                            case 2: return ItemSort.DamageArtifact;
                            case 3: return ItemSort.HorseWhip;
                        }
                        break;
                    }
                case 3:
                    {
                        if (typeId / 10 * 10 == typeId)
                            return ItemSort.Mount;

                        switch (typeId % 1000000 / 1000)
                        {
                            case 350: return ItemSort.Weapon2Coat;
                            case 360: return ItemSort.Weapon1Coat;
                            case 370: return ItemSort.BowCoat;
                            case 380: return ItemSort.ShieldCoat;
                        }
                        break;
                    }
                case 4: return ItemSort.Weapon1;
                case 5: return ItemSort.Weapon2;
                case 6: return ItemSort.Weapon1;
                case 7: return ItemSort.Other;
                case 9: return ItemSort.Shield;
                case 10: return ItemSort.Expendable;
                default:
                    {
                        break;
                    }
            }

            var sort = typeId % 10000000 / 100000;
            if (sort >= 20 && sort < 30) return ItemSort.RingL;

            return ItemSort.Invalid;
        }

        public static ItemPosition GetItemPosition(ItemSort sort, ItemType subType = ItemType.Invalid)
        {
            var pos = ItemPosition.Inventory;
            switch (sort)
            {
                case ItemSort.Helmet:
                    pos = ItemPosition.Set1Helmet;
                    break;

                case ItemSort.Necklace:
                    pos = ItemPosition.Set1Necklace;
                    break;

                case ItemSort.Armor:
                    pos = ItemPosition.Set1Armor;
                    break;

                case ItemSort.Weapon1:
                    pos = ItemPosition.Set1Weapon1;
                    break;

                case ItemSort.Weapon2:
                    pos = ItemPosition.Set1Weapon1;
                    break;

                case ItemSort.Shield:
                    pos = ItemPosition.Set1Weapon2;
                    break;

                case ItemSort.RingR:
                    pos = ItemPosition.Set1Ring;
                    break;

                case ItemSort.Shoes:
                    pos = ItemPosition.Set1Boots;
                    break;

                case ItemSort.RingL:
                    pos = ItemPosition.Set1Gourd;
                    break;

                case ItemSort.Overcoat:
                    pos = ItemPosition.Set1Garment;
                    break;

                case ItemSort.DamageArtifact:
                    {
                        switch (subType)
                        {
                            case ItemType.IncreaseDmgArtifact:
                                pos = ItemPosition.Fan;
                                break;

                            case ItemType.DecreaseDmgArtifact:
                                pos = ItemPosition.Tower;
                                break;
                        }
                        break;
                    }
                case ItemSort.Mount:
                    pos = ItemPosition.Steed;
                    break;

                case ItemSort.MountDecorator:
                    pos = ItemPosition.SteedAccessory;
                    break;

                case ItemSort.HorseWhip:
                    pos = ItemPosition.Crop;
                    break;

                case ItemSort.Weapon1Coat:
                    pos = ItemPosition.W1Accessory;
                    break;
                case ItemSort.Weapon2Coat:
                case ItemSort.BowCoat:
                    pos = ItemPosition.W1Accessory;
                    break;

                case ItemSort.ShieldCoat:
                    pos = ItemPosition.W2Accessory;
                    break;
            }
            return pos;
        }

        public GameClient? Owner { get; set; }

        public int Id { get; set; } //per unique instance
        public int TypeId { get; set; } //actual item# /id
        public int Durability { get; set; }
        public int MaxDurability { get; set; }
        public ItemPosition Position { get; set; }
        public int AltProgress { get; set; }
        public int Gem1 { get; set; }
        public int Gem2 { get; set; }
        public int WeaponAttribute { get; set; }
        public int Composition { get; set; }
        public int Bless { get; set; }
        public int LockStatus { get; set; }
        public int Enchant { get; set; } //health added to player
        public int Color { get; set; }
        public int CompositionProgress { get; set; }
        public TimeSpan? Duration { get; set; }
        public int Stack { get; set; }

        //extended properties
        public int OwnerId { get { return Owner != null ? Owner.Id : 0; } }
        public int Cost { get; set; }
        public ItemSort Sort { get { return GetSort(TypeId); } }
        public ItemType SubType { get { return GetSubType(TypeId); } }
        public ItemPosition EquipPosition { get { return GetItemPosition(Sort, SubType); } }


        private ItemTypeModel? _attributes;
        public ItemTypeModel Attributes
        {
            get
            {
                if (_attributes == null || _attributes.TypeId != TypeId)
                {
                    _attributes = Db.GetItemTypeByTypeId(TypeId);
                }
                return _attributes;
            }
        }

        public Item()
        {

        }

        public Item(ItemModel model, GameClient client)
        {
            Owner = client;

            Id = model.Id;
            TypeId = model.TypeId;
            Durability = model.Durability;
            MaxDurability = model.MaxDurability;
            Position = model.Position;

            AltProgress = model.AltProgress;
            Gem1 = model.Gem1;
            Gem2 = model.Gem2;
            WeaponAttribute = model.WeaponAttribute;
            Composition = model.Composition;

            Bless = model.Bless;
            LockStatus = model.LockStatus;
            Enchant = model.Enchant;
            Color = model.Color;
            CompositionProgress = model.CompositionProgress;

            Duration = model.Duration;
            Stack = model.Stack;
            Cost = model.Cost;
        }

    }
}
