using ConquerServer.Client;
using ConquerServer.Shared;
using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database.Models
{
    public class ItemModel
    {
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
        public int Enchant { get; set; }
        public int Color { get; set; }
        public int CompositionProgress { get; set; }


        public TimeSpan? Duration { get; set; }
        public int Stack { get; set; }

        //extended properties
        public int OwnerId { get; set; }
        public int Cost { get; set; }


        public ItemModel() 
        {
            
        }


        public ItemModel(Item item)
        {
            Id = item.Id;
            TypeId = item.TypeId;
            Durability = item.Durability;
            MaxDurability = item.MaxDurability;
            Position = item.Position;

            AltProgress = item.AltProgress;
            Gem1 = item.Gem1;
            Gem2 = item.Gem2;
            WeaponAttribute = item.WeaponAttribute;
            Composition = item.Composition;

            Bless = item.Bless;
            LockStatus = item.LockStatus;
            Enchant = item.Enchant;
            Color = item.Color;
            CompositionProgress = item.CompositionProgress;

            Duration = item.Duration;
            Stack = item.Stack;
            OwnerId = item.OwnerId;
        }
    }
}
