using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using ConquerServer.Client;

namespace ConquerServer.Database.Models
{
    public class CharacterModel
    {
        /*this is a roberry, put your hands up *gun pointed at gameclientCLASS*    */
        public int Id { get; set; }
        public Lookface Lookface { get; set; }
        public short HairStyle { get; set; }
        public int Gold { get; set; }
        public int ConquerPoints { get; set; }
        public long Experience { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Spirit { get; set; }
        public int AttributePoints { get; set; } //floating stat points
        public int Health { get; set; }
        public int Mana { get; set; }
        public int PKPoints { get; set; }
        public int Level { get; set; }
        public int Job { get; set; }
        public int Rebirth { get; set; }
        public int QuizPoints { get; set; }
        public int EnlightenPoints { get; set; }
        public int VipLevel { get; set; }
        public int SubProfessionList { get; set; }
        public int Nationality { get; set; }
        public string Name { get; set; }
        public string SpouseName { get; set; }

        public int MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public List<ItemModel> Inventory { get; set; }
        public List<ItemModel> Equipment { get; set; }
        public List<MagicModel> Magics { get; set; }

        public CharacterModel()
        {
            //JSON needs a default constructor
            //public sets.. fuck does this mean i need a list to reassign QQ
            Name = string.Empty;
            SpouseName = string.Empty;
            Inventory = new List<ItemModel>();
            Equipment = new List<ItemModel>();
            Magics = new List<MagicModel>();
        }

        public CharacterModel(GameClient client)
        {
            //ok so the robbery shold be completed now, maybe this will need to move later, idk yet
            this.Id = client.Id;
            this.Lookface = client.Lookface;
            this.HairStyle = client.HairStyle;
            this.Gold = client.Gold;
            this.ConquerPoints = client.ConquerPoints;

            this.Experience = client.Experience;
            this.Strength = client.Strength;
            this.Agility = client.Agility;
            this.Vitality = client.Vitality;
            this.Spirit = client.Spirit;

            this.AttributePoints = client.AttributePoints;
            this.Health = client.Health;
            this.Mana = client.Mana;
            this.PKPoints = client.PKPoints;
            this.Level = client.Level;

            this.Job = client.Job;
            this.Rebirth = client.Rebirth;
            this.QuizPoints = client.QuizPoints;
            this.EnlightenPoints = client.EnlightenPoints;
            this.VipLevel = client.VipLevel;

            this.SubProfessionList = client.SubProfessionList;
            this.Nationality = client.Nationality;
            this.Name = client.Name;
            this.SpouseName = client.SpouseName;

            this.MapId = client.MapId;
            this.X = client.X;
            this.Y = client.Y;
            
            this.Inventory = client.Inventory.Select(i => new ItemModel(i)).ToList();
            this.Equipment = client.Equipment.Select(i => new ItemModel(i)).ToList();
            this.Magics = client.Magics.Values .Select(i => new MagicModel(i)).ToList();
        }
        
      
    }
}
