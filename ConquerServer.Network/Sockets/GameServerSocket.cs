using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network.Cryptography;

namespace ConquerServer.Network.Sockets
{
    public class GameServerSocket : ServerSocket
    {
        private static readonly byte[] DefaultKey = Encoding.ASCII.GetBytes("C238xs65pjy7HU9Q");
        private static readonly byte[] Padding = Encoding.ASCII.GetBytes("TQServer");

        public GameServerSocket(int port)
            : base(port)
        {

        }

        protected override bool OnClientMessage(ClientSocket client, Packet message)
        {
            if (client.State != null)
            {
                var handshake = client.State as Handshake;
                if (handshake != null)
                {
                    var cipher = (CastCipher)client.Cipher;
                    cipher.SetKey(handshake.ComputeKey(message));
                    cipher.EncIvec = new byte[8];
                    cipher.DecIvec = new byte[8];

                    Console.WriteLine("Completed handshake");

                    // set things back to normal, and trigger regular ClientConnected
                    client.State = null;
                    client.HeaderSize = 2;
                    RaiseClientConnected(client);

                    // do not let this message pass through to ClientMessage
                    return false;
                }
            }

            //Console.WriteLine(message.Dump());

            return true;
        }

        protected override bool OnClientConnected(ClientSocket client)
        {
            var handshake = new Handshake();
            client.State = handshake;
            client.HeaderSize = handshake.HeaderSize;
            client.ExpectedMessageSize = handshake.HeaderSize;
            client.Padding = Padding;

            //Console.WriteLine("Game Client connected, requesting handshake...");
            using (var p = handshake.CreateRequest())
                client.Send(p);

            // do not fire the typical ClientConnected event
            return false;
        }

        protected override ICipher CreateCipher()
        {
            var cipher = new CastCipher();
            cipher.SetKey(DefaultKey);
            return cipher;
        }
    }
}
