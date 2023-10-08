using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;

namespace ConquerServer.Network.Packets
{
    public class SpellInfoPacket : Packet
    {
        public uint Timestamp { get; set; }
        public uint Experience { get; set; }
        public ushort TypeId { get; set; }
        public ushort Level { get; set; }
        public short Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public int Unknown3 { get; set; }

        public SpellInfoPacket(ISkill magic)
            : base(32)
        {
            Timestamp = TimeStamp.GetTime();
            Experience = (uint)magic.Experience;
            TypeId = (ushort)magic.TypeId;
            Level = (ushort)magic.Level;
            Unknown1 = 0;
            Unknown2 = 0;
            Unknown3 = 0;

            Build();
        }

        public SpellInfoPacket(Packet p)
            : base(32)
        {
            Timestamp = p.ReadUInt32();
            Experience = p.ReadUInt32();
            TypeId = p.ReadUInt16();
            Level = p.ReadUInt16();
            Unknown1 = p.ReadInt16();
            Unknown2 = p.ReadInt16();
            Unknown3= p.ReadInt32();
        }

        public override void Build()
        {
            var p = this;
            p.WriteUInt32((uint)Timestamp); // 5735
            p.WriteUInt32((uint)Experience);
            p.WriteUInt16((ushort)TypeId);
            p.WriteUInt16((ushort)Level);
            p.WriteInt16((short)Unknown1);
            p.WriteInt16((byte)Unknown2);
            p.WriteInt32((int)Unknown2);
            p.Build(PacketType.MagicInfo);
        }
    }
}
