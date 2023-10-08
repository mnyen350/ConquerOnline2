using ConquerServer.Database.Models;
using ConquerServer.Database;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network.Packets;
using System.Reflection.PortableExecutable;

namespace ConquerServer.Client
{
    public abstract class Skill : ISkill
    {
        public GameClient Owner { get; set; }
        public long Experience { get; set; }
        public int TypeId { get; private set; }
        public int Level { get; set; }

        public Skill(GameClient client, int typeId, int level = 0, long experience = 0)
        {
            Owner = client;
            Experience = experience;
            TypeId = typeId;
            Level = level;
        }

        public abstract void Send();
    }

    public class Magic : Skill
    {
        private MagicTypeModel? _attributes;

        public MagicTypeModel Attributes
        {
            get
            {
                //typeid + level for composite key
                if (_attributes == null || _attributes.Type == TypeId || _attributes.Level == Level)
                {
                    _attributes = Db.GetMagicType(TypeId, Level);
                }
                return _attributes;
            }
        }
        public Magic(GameClient client, int typeId, int level = 0, long experience = 0)
            :base(client, typeId, level, experience)
        {
        }

        public override void Send()
        {
            using (var sip = new SpellInfoPacket(this))
                Owner.Send(sip);
        }
    }

    public class Proficiency : Skill
    {
        private static readonly long[] NEED_EXPERIENCE =
        {
            0,
            1200,
            68000,
            250000,
            640000,
            1600000,
            4000000,
            10000000,
            22000000,
            40000000,
            90000000,
            95000000,
            142500000,
            213750000,
            320625000,
            480937500,
            721406250,
            1082109375,
            1623164063,
            2100000000
        };

        public ProficiencyType Type 
        { 
            get { return (ProficiencyType)TypeId; }
        }

        public long NeedExperience
        {
            get
            {
                if(Level>= NEED_EXPERIENCE.Length) return 0;
                else return NEED_EXPERIENCE[Level-1];
            }
        }
         
        public Proficiency(GameClient client, ProficiencyType type, int level = 0, long experience = 0)
            : base(client, (int)type, level, experience)
        {

        }

        public override void Send()
        {
            using (var pip = new ProficiencyInfoPacket(this, NeedExperience))
                Owner.Send(pip);
        }
    }
}
