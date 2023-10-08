using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Packets
{
    public class LoadMapPacket :Packet
    {
        public uint Timestamp { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int MapId { get; set; }

        public LoadMapPacket(int mapId) 
            : base(20)
        {
            Timestamp = TimeStamp.GetTime();
            Unknown1 = 0;
            Unknown2 = 1;
            MapId = mapId;
            Build();
        }

        public LoadMapPacket(Packet p)
            :base (20)
        {
            Timestamp = p.ReadUInt32();
            Unknown1 = p.ReadInt32();
            Unknown2 = p.ReadInt32();
            MapId = p.ReadInt32();

        }

        public override void Build()
        {
            WriteUInt32((uint)Timestamp); // 5735
            WriteInt32(Unknown1);
            WriteInt32(Unknown2);
            WriteInt32(MapId);
            
            Build(PacketType.LoadMap);
        }
    }
}
