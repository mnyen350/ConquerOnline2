using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ConquerServer.Network.Packets
{
    public class ItemUsePacket: Packet
    {
        public uint Timestamp { get; set; }
        public uint Timestamp2 { get; set; }
        public int Id { get; set; } //item id? playerid? typeid? 
        public int Data { get; set; }
        public ItemAction Action { get ; set; }
        public int[] Values { get; set; }

        public ItemUsePacket(ItemAction action, int id, int data = 0, params int[] values)
            :base(128)
        {
            if (values.Length > 0 && values.Length != 16)
                throw new ArgumentException(nameof(values), $"If {nameof(values)} parameter is specified, it must have 16 values");

            Timestamp = TimeStamp.GetTime();
            Timestamp2 = TimeStamp.GetTime();
            Id = id;
            Data = data;
            Action = action;
            Values = values;

            Build();
        }

        public ItemUsePacket(Packet p)
            : base(128)
        {
            Timestamp = p.ReadUInt32();
            Id = p.ReadInt32();
            Data = p.ReadInt32();
            Action = (ItemAction)p.ReadInt32();
            Timestamp2 = p.ReadUInt32();

            int lengthOfValues = (p.Size - p.Offset) / sizeof(int);
            Values = new int[lengthOfValues];
            for (int i = 0; i < Values.Length; i++)
                Values[i] = p.ReadInt32();

        }

        public override void Build()
        {
            WriteUInt32(Timestamp); // 5735
            WriteInt32(Id);
            WriteInt32(Data);
            WriteInt32((int)Action);
            WriteUInt32(Timestamp2);
            for (int i = 0; i < Values.Length; i++)
                WriteInt32(Values[i]);
            Build(PacketType.Item);
        }
    }
}
