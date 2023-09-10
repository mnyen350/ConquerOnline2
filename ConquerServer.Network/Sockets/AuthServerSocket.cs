using ConquerServer.Network.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Sockets
{
    public class AuthServerSocket : ServerSocket
    {
        public AuthServerSocket(int port)
            : base(port)
        {

        }

        protected override ICipher CreateCipher()
        {
            return new AuthCipher();
        }

        protected override bool OnClientConnected(ClientSocket client)
        {
            // need to reply to the login here...
            using (var p = new Packet(8))
            {
                p.WriteInt32(0x1337dead);
                p.Build(PacketType.EncryptCode);
                client.Send(p);
            }

            return true;
        }
    }
}
