using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Packets
{
    public class ActionPacket :Packet
    {
        public uint Timestamp { get; set; }
        public int Id { get; set; }
        public long Data { get; set; }
        public uint Timestamp2 { get; set; }
        public ActionType Action { get; set; }
        public Direction Direction { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public bool Charging { get; set; }
        public string[] Strings { get; set; }

        public ActionPacket(int id, int x, int y, Direction direction, ActionType action, long data)
            : base(64)
        {
            Timestamp = TimeStamp.GetTime();
            Id = id;
            Data = data;
            Timestamp2 = TimeStamp.GetTime();
            Action = action;
            Direction = direction;
            X = x;
            Y = y;
            Unknown1 = 0;
            Unknown2 = 0;
            Charging = false;
            Strings = new string[0];

            Build();
        }

        public ActionPacket(Packet p)
            :base(64)
        {
            Timestamp = p.ReadUInt32();
            Id = p.ReadInt32();
            Data = p.ReadInt64();
            Timestamp2 = p.ReadUInt32();
            Action = (ActionType)p.ReadInt16();
            Direction = (Direction)p.ReadInt16();
            X = p.ReadInt16();
            Y = p.ReadInt16();
            Unknown1 = p.ReadInt32();
            Unknown2 = p.ReadInt32();
            Charging = p.ReadUInt8() != 0;
            Strings = p.ReadStrings();
        }

        public override void Build()
        {
            WriteUInt32(Timestamp);                 // 4
            WriteInt32(Id);                         // 8
            WriteInt64(Data);                       // 12
            WriteUInt32(Timestamp2);                // 20
            WriteInt16((short)Action);              // 24
            WriteInt16((short)Direction);           // 26
            WriteInt16((short)X);                   // 28
            WriteInt16((short)Y);                   // 30
            WriteInt32(Unknown1);                   // 32
            WriteInt32(Unknown2);                   // 36
            WriteInt8((byte)(Charging ? 1 : 0));    // 40
            WriteStrings(Strings);                  // 41

            Build(PacketType.Action);
        }

    }
}
