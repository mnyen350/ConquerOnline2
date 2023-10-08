using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Packets
{
    public class ServerTimeInfoPacket :Packet
    {
        public DateTime Now { get; set; }


        public ServerTimeInfoPacket() 
            :base(40)
        {
            Now = DateTime.Now; // local time
            Build();
        }

        public ServerTimeInfoPacket(Packet p)
            :base(40)
        {
            uint timestamp = ReadUInt32();
            Now = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            // ignore rest
        }

        public override void Build()
        {
            long timestamp = ((DateTimeOffset)Now).ToUnixTimeSeconds();

            WriteUInt32((uint)timestamp); // 5735
            WriteInt32(0); // type
            WriteInt32(Now.Year - 1900);
            WriteInt32(Now.Month - 1);
            WriteInt32(Now.DayOfYear);
            WriteInt32(Now.Day);
            WriteInt32(Now.Hour);
            WriteInt32(Now.Minute);
            WriteInt32(Now.Second);

            Build(PacketType.Data);
        }
    }
}
