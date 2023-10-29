using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database.Models
{
    public enum MagicTargetType : int
    {
        Ghost = 64,
        Passive = 8,
        Ground = 4,
        Self = 2,
        All = 1,
        Enemy = 0
    }

    public enum MagicSort
    {
        Attack = 1,
        Recruit = 2,
        Cross = 3,
        Fan = 4,
        Bomb = 5,
        AttachStatus = 6,
        DetachStatus = 7,
        Square = 8,
        JumpAttack = 9,
        RandomTransport = 10,
        DispatchXp = 11,
        Collide = 12,
        SerialCut = 13,
        Line = 14,
        AtkRange = 15,
        AttackStatus = 16,
        CallTeamMember = 17,
        RecordTransportSpell = 18,
        Transform = 19,
        AddMana = 20,
        LayTrap = 21,
        Dance = 22,
        CallPet = 23,
        Vampire = 24,
        Instead = 25,
        DecLife = 26,
        BombGroundTarget = 27,
        AttachShurikenVortex = 28,
        Mount = 32,
        GibbonAttack = 34,
        MoveThickLine = 40,
        TargetedLine = 48,
        AuraToggle = 51,
        AttachOblivion = 53,
        BombStun = 54,
        MoveLine = 61,
        MoveDrag = 65,

        // custom
        UtilityShot = 100
    }

    public class MagicTypeModel
    {
        public static int CreateCompositeKey(int type, int level) => type * 10 + level;
        public int CompositeKey { get { return CreateCompositeKey(this.Type, this.Level); } }

        public int Id { get; set; }
        public int Type { get; set; }
        public MagicSort Sort { get; set; }
        public string Name { get; set; }
        public int Offensive { get; set; }
        /// ///////////////
        public int Ground { get; set; }
        public int Unknown1 { get; set; }
        public MagicTargetType Target { get; set; } //make type the enum?
        public int Level { get; set; }
        public int UseMP { get; set; }
        /// /// ///////////////
        public int Power { get; set; }
        public int DelayCast { get; set; }
        public int Accuracy { get; set; }
        public int StepSecond { get; set; }
        public int Range { get; set; }
        /// /// ///////////////
        public int Distance { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        /// /// ///////////////
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public ItemType WeaponSubType { get; set; }
        public int Unknown9 { get; set; }
        public int Unknown10 { get; set; }
        /// /// ///////////////
        public int Unknown11 { get; set; }
        public int Unknown12 { get; set; }
        public int Unknown13 { get; set; }
        public int Unknown14 { get; set; }
        public int UseStamina { get; set; }
        /// /// ///////////////
        public bool IsWeaponPassive { get; set; }
        public int UseItem { get; set; }
        public int NextMagic { get; set; }
        public int DelayMS { get; set; }
        /// ///////////////
        public int Unknown15 { get; set; }
        public int Unknown16 { get; set; }
        public int Unknown17 { get; set; }
        public int Unknown18 { get; set; }
        public int Unknown19 { get; set; }
        /// ///////////////
        public int Unknown20 { get; set; }
        public int Unknown21 { get; set; }
        public int Unknown22 { get; set; }
        public int Unknown23 { get; set; }
        public int Unknown24 { get; set; }
        /// ///////////////
        public int Unknown25 { get; set; }
        public int Unknown26 { get; set; }
        public int Unknown27 { get; set; }
        public int Unknown28 { get; set; }
        public int DelayNextMagic { get; set; }

        public MagicTypeModel()
        {
            Name = string.Empty;
        }
        public static MagicTypeModel Parse(string s)
        {
            string[] split = s.Split(' ');
            MagicTypeModel model = new MagicTypeModel();

            model.Id = int.Parse(split[0]);
            model.Type = int.Parse(split[1]);
            model.Sort = (MagicSort)int.Parse(split[2]);
            model.Name = split[3];
            model.Offensive = int.Parse(split[4]);

            model.Ground = int.Parse(split[5]);
            model.Unknown1 = int.Parse(split[6]);
            model.Target = (MagicTargetType)int.Parse(split[7]);
            model.Level = int.Parse(split[8]);
            model.UseMP = int.Parse(split[9]);

            model.Power = int.Parse(split[10]);
            model.DelayCast = int.Parse(split[11]);
            model.Accuracy = int.Parse(split[12]);
            model.StepSecond = int.Parse(split[13]);
            model.Range = int.Parse(split[14]);

            model.Distance = int.Parse(split[15]);
            model.Unknown3 = int.Parse(split[16]);
            model.Unknown4 = int.Parse(split[17]);
            model.Unknown5 = int.Parse(split[18]);
            model.Unknown6 = int.Parse(split[19]);

            model.Unknown7 = int.Parse(split[20]);
            model.Unknown8 = int.Parse(split[21]);
            model.WeaponSubType = (ItemType)int.Parse(split[22]);
            model.Unknown9 = int.Parse(split[23]);
            model.Unknown10 = int.Parse(split[24]);

            model.Unknown11 = int.Parse(split[25]);
            model.Unknown12 = int.Parse(split[26]);
            model.Unknown13 = int.Parse(split[27]);
            model.Unknown14 = int.Parse(split[28]);
            model.UseStamina = int.Parse(split[29]);

            model.IsWeaponPassive = int.Parse(split[30]) != 0;
            model.UseItem = int.Parse(split[31]);
            model.NextMagic = int.Parse(split[32]);
            model.DelayMS = int.Parse(split[33]);

            model.Unknown15 = int.Parse(split[34]);
            model.Unknown16 = int.Parse(split[35]);
            model.Unknown17 = int.Parse(split[36]);
            model.Unknown18 = int.Parse(split[37]);
            model.Unknown19 = int.Parse(split[38]);

            model.Unknown20 = int.Parse(split[39]);
            model.Unknown21 = int.Parse(split[40]);
            model.Unknown22 = int.Parse(split[41]);
            model.Unknown23 = int.Parse(split[42]);
            model.Unknown24 = int.Parse(split[43]);

            model.Unknown25 = int.Parse(split[44]);
            model.Unknown26 = int.Parse(split[45]);
            model.Unknown27 = int.Parse(split[46]);
            model.Unknown28 = int.Parse(split[47]);
            model.DelayNextMagic = int.Parse(split[48]);

            return model;
        }
    }
}
