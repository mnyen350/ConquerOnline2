using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Packets
{
    public class ProficiencyInfoPacket : Packet
    {
        public int TypeId { get; set; }
        public int Level { get; set; }
        public uint Experience { get; set; }
        public uint NeedExperience { get; set; }
        
        public ProficiencyInfoPacket(ISkill proficiency, long needExperience) 
            : base(32)
        {
            TypeId = proficiency.TypeId;
            Level = proficiency.Level;
            Experience = (uint)proficiency.Experience;
            NeedExperience = (uint)needExperience;

            Build();
        }

        public ProficiencyInfoPacket(Packet p)
            : base(32)
        {
            TypeId = p.ReadInt32();
            Level = p.ReadInt32();
            Experience = p.ReadUInt32();
            NeedExperience = p.ReadUInt32();
        }

        public override void Build()
        {
            WriteInt32(TypeId);
            WriteInt32(Level);
            WriteUInt32((uint)Experience);
            WriteUInt32((uint)NeedExperience);
            
            Build(PacketType.WeaponSkill);
        }
    }
}
