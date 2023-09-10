using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class NetworkAttribute : Attribute, IDispatcherAttribute<PacketType>
    {
        public PacketType Key { get; private set; }
        public NetworkAttribute(PacketType type)
        {
            Key = type;
        }
    }
}
