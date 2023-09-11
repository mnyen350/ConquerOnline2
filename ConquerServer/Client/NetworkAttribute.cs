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
        public PacketType[] Keys { get; private set; }
        public NetworkAttribute(params PacketType[] types)
        {
            Keys = types;
        }
    }
}
