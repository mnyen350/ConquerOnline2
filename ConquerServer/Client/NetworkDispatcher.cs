using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class NetworkDispatcher<TClient> 
        : Dispatcher<PacketType, NetworkAttribute, Func<TClient, Packet, Task>>
    {
        public NetworkDispatcher()
            : base(typeof(TClient))
        {

        }
    }
}
