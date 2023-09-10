using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;

namespace ConquerServer.Database.Models
{
    public class ItemTypeModel
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        public int RequiredJob { get; set; }
        public int RequiredProficiencyLevel { get; set; }
        public int RequiredLevel { get; set; }
        public int RequiredSex { get; set; }
        
        //////////////
        public int RequiredStregnth { get; set; }
        public int RequiredAgility { get; set; }
        public int RequiredVitality { get; set; }
        public int RequiredSpirit { get; set; }
        
        //////////////
        public int Tradability { get; set; }
        public int Weight { get; set; }
        public int ShopBuyPrice { get; set; }
        //////////////
        
        public int Action { get; set; }
        public int MaximumPhysicalAttack { get; set; }
        public int MinimumPhysicalAttack { get; set; }
        public int PhysicalDefense { get; set; }
        ////////////////
       
        public int Dexterity { get; set; }
        public int Dodge { get; set; }
        public int PotionAddHP { get; set; }
        public int PotionAddMP { get; set; }
        public int Durabiliy { get; set; }
        ///////////////
       
        public int Arrows { get; set; }
        public int Identity { get; set; }
        public int Gem1 { get; set; }
        public int Gem2 { get; set; }
        ///////////////
        
        public int Magic1 { get; set; }
        public int Magic2 { get; set; }
        public int Magic3 { get; set; }
        public int Unknown1 { get; set; }
        public int MagicAttack { get; set; }
        public int MagicDefense { get; set; }
        ///////////////////
       
        public int Range { get; set; }
        public int AttackSpeed { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        ////////////////
        
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown9 { get; set; }
        public int Unknown10 { get; set; }
        public int Unknown11 { get; set; }
        public int Unknown12 { get; set; }
        public int Unknown13 { get; set; }
        public int Unknown14 { get; set; }
        public int Unknown15 { get; set; }
        public int Unknown16 { get; set; }
        public int Unknown17 { get; set; }
        public int Unknown18 { get; set; }
        public int Unknown19 { get; set; }
        //////////////////////
        public int ShopCPPrice { get; set; }
        public string Description { get; set; }
        public string Properties { get; set; }
        
        /// ///////////////
        public int Unknown20 { get; set; }
        public int Unknown21 { get; set; }
        public int Unknown22 { get; set; }
        public int Unknown23 { get; set; }
        public int Unknown24 { get; set; }
        public int Unknown25 { get; set; }


        public ItemTypeModel()
        {
            Name = string.Empty; 
            Description = string.Empty;
            Properties = string.Empty;
        }

        public static ItemTypeModel Parse(string s)
        {
            string[] splitStr = s.Split(' ');
            var r = new ItemTypeModel();

            r.TypeId = int.Parse(splitStr[0]);
            r.Name = splitStr[1];
            r.RequiredJob = int.Parse(splitStr[2]);
            r.RequiredProficiencyLevel = int.Parse(splitStr[3]);
            r.RequiredLevel = int.Parse(splitStr[4]);
            r.RequiredSex = int.Parse(splitStr[5]);
            r.RequiredStregnth = int.Parse(splitStr[6]);
            r.RequiredAgility = int.Parse(splitStr[7]);
            r.RequiredVitality = int.Parse(splitStr[8]);
            r.RequiredSpirit = int.Parse(splitStr[9]);
            r.Tradability = int.Parse(splitStr[10]);
            r.Weight = int.Parse(splitStr[11]);
            r.ShopBuyPrice = int.Parse(splitStr[12]);
            r.Action = int.Parse(splitStr[13]);
            r.MaximumPhysicalAttack = int.Parse(splitStr[14]);
            r.MinimumPhysicalAttack = int.Parse(splitStr[15]);
            r.PhysicalDefense = int.Parse(splitStr[16]);
            r.Dexterity = int.Parse(splitStr[17]);
            r.Dodge = int.Parse(splitStr[18]);
            r.PotionAddHP = int.Parse(splitStr[19]);
            r.PotionAddMP = int.Parse(splitStr[20]);
            r.Durabiliy = int.Parse(splitStr[21]);
            r.Arrows = int.Parse(splitStr[22]);
            r.Identity = int.Parse(splitStr[23]);
            r.Gem1 = int.Parse(splitStr[24]);
            r.Gem2 = int.Parse(splitStr[25]);
            r.Magic1 = int.Parse(splitStr[26]);
            r.Magic2 = int.Parse(splitStr[27]);
            r.Magic3 = int.Parse(splitStr[28]);
            r.Unknown1 = int.Parse(splitStr[29]);
            r.MagicAttack = int.Parse(splitStr[30]);
            r.MagicDefense = int.Parse(splitStr[31]);
            r.Range = int.Parse(splitStr[32]);
            r.AttackSpeed = int.Parse(splitStr[33]);
            r.Unknown2 = int.Parse(splitStr[34]);
            r.Unknown3 = int.Parse(splitStr[35]);
            r.Unknown4 = int.Parse(splitStr[36]);
            r.Unknown5 = int.Parse(splitStr[37]);
            r.Unknown6 = int.Parse(splitStr[38]);
            r.Unknown7 = int.Parse(splitStr[39]);
            r.Unknown8 = int.Parse(splitStr[40]);
            r.Unknown9 = int.Parse(splitStr[41]);
            r.Unknown10 = int.Parse(splitStr[42]);
            r.Unknown11 = int.Parse(splitStr[43]);
            r.Unknown12 = int.Parse(splitStr[44]);
            r.Unknown13 = int.Parse(splitStr[45]);
            r.Unknown14 = int.Parse(splitStr[46]);
            r.Unknown15 = int.Parse(splitStr[47]);
            r.Unknown16 = int.Parse(splitStr[48]);
            r.Unknown17 = int.Parse(splitStr[49]);
            r.Unknown18 = int.Parse(splitStr[50]);
            r.Unknown19 = int.Parse(splitStr[51]);
            r.ShopCPPrice = int.Parse(splitStr[52]);
            r.Description = splitStr[53];
            r.Properties = splitStr[54];
            r.Unknown20 = int.Parse(splitStr[55]);
            r.Unknown21 = int.Parse(splitStr[56]);
            r.Unknown22 = int.Parse(splitStr[57]);
            r.Unknown23 = int.Parse(splitStr[58]);
            r.Unknown24 = int.Parse(splitStr[59]);
            r.Unknown25 = int.Parse(splitStr[60]);

            return r;
        }
    }
}
