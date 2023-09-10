using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono2.Math;
using System.Security.Cryptography;
using ConquerServer.Network.Cryptography;

namespace ConquerServer.Network.Sockets
{
    internal class Handshake
    {
        private const string PStr = "A320A85EDD79171C341459E94807D71D39BB3B3F3B5161CA84894F3AC3FC7FEC317A2DDEC83B66D30C29261C6492643061AECFCF4A051816D7C359A6A7B7D8FB";
        private const string GStr = "05";

        public BigInteger G { get; private set; }
        public BigInteger P { get; private set; }
        public BigInteger PublicKey { get; private set; }
        public BigInteger PrivateKey { get; private set; }
        public int HeaderSize { get { return 9; } }

        public Handshake()
        {
            P = BigInteger.ParseHex(PStr);
            G = BigInteger.ParseHex(GStr);
            PrivateKey = BigInteger.GeneratePseudoPrime(40 * 8);
            PublicKey = BigInteger.ModPow(G, PrivateKey, P);
        }

        public Packet CreateRequest()
        {
            /*
             *  Packet - Size:[331] Source:[ToClient] Type:[57415]
                [000] 03 d6 47 e0 b7 73 99 12 85 0f 4f 48 01 00 00 12 	..G..s....OH....
                [016] 00 00 00 77 7a ec a2 e5 33 90 54 b7 61 c4 c2 61 	...wz...3.T.a..a
                [032] d8 7c ce 90 cd 08 00 00 00 1e e0 c9 9e f3 3e 79 	.|............>y
                [048] af 08 00 00 00 71 4d c8 30 5f d8 17 4e 80 00 00 	.....qM.0_..N...
                [064] 00 41 33 32 30 41 38 35 45 44 44 37 39 31 37 31 	.A320A85EDD79171
                [080] 43 33 34 31 34 35 39 45 39 34 38 30 37 44 37 31 	C341459E94807D71
                [096] 44 33 39 42 42 33 42 33 46 33 42 35 31 36 31 43 	D39BB3B3F3B5161C
                [112] 41 38 34 38 39 34 46 33 41 43 33 46 43 37 46 45 	A84894F3AC3FC7FE
                [128] 43 33 31 37 41 32 44 44 45 43 38 33 42 36 36 44 	C317A2DDEC83B66D
                [144] 33 30 43 32 39 32 36 31 43 36 34 39 32 36 34 33 	30C29261C6492643
                [160] 30 36 31 41 45 43 46 43 46 34 41 30 35 31 38 31 	061AECFCF4A05181
                [176] 36 44 37 43 33 35 39 41 36 41 37 42 37 44 38 46 	6D7C359A6A7B7D8F
                [192] 42 02 00 00 00 30 35 80 00 00 00 30 43 36 30 36 	B....05....0C606
                [208] 34 43 39 45 39 35 31 34 36 32 33 43 30 33 32 34 	4C9E9514623C0324
                [224] 45 36 43 44 39 36 35 32 44 31 33 44 35 46 37 41 	E6CD9652D13D5F7A
                [240] 39 37 35 43 32 38 36 46 42 33 45 31 36 41 34 42 	975C286FB3E16A4B
                [256] 45 31 38 43 41 36 34 39 42 34 36 37 44 39 37 35 	E18CA649B467D975
                [272] 30 44 39 39 30 36 35 38 34 42 36 41 36 42 38 41 	0D9906584B6A6B8A
                [288] 33 42 30 34 38 35 33 35 46 44 32 35 33 42 30 36 	3B048535FD253B06
                [304] 30 43 31 32 46 38 46 43 35 36 30 30 46 34 45 42 	0C12F8FC5600F4EB
                [320] 44 41 32 31 37 32 41 34 45 36 33                	DA2172A4E63     
            */
            var pStr = Encoding.UTF8.GetBytes(PStr);
            var gStr = Encoding.UTF8.GetBytes(GStr);
            var pubStr = Encoding.UTF8.GetBytes(PublicKey.ToString("X"));

            var p = new Packet(512);
            p.Offset = 0;
            p.WriteBytes(new byte[] { 0x03, 0xd6, 0x47, 0xe0, 0xb7, 0x73, 0x99, 0x12, 0x85, 0x0f, 0x4f });
            p.WriteInt32(0); //Size

            p.WriteInt32(0x12); //Random junk, again.
            p.WriteBytes(new byte[] { 0x77, 0x7a, 0xec, 0xa2, 0xe5, 0x33, 0x90, 0x54, 0xb7, 0x61, 0xc4, 0xc2, 0x61, 0xd8, 0x7c, 0xce, 0x90, 0xcd });

            //Ivecs
            p.WriteInt32(8);
            p.Fill(8);
            p.WriteInt32(8);
            p.Fill(8);

            p.WriteInt32(pStr.Length);
            p.WriteBytes(pStr);
            p.WriteInt32(gStr.Length);
            p.WriteBytes(gStr);
            p.WriteInt32(pubStr.Length);
            p.WriteBytes(pubStr);
            p.Build();
            p.WriteInt32(11, p.Offset - 3);
            return p;
        }

        public byte[] ComputeKey(Packet message)
        {
            message.Offset = 0;
            message.Skip(11); //Junk
            message.Skip(message.ReadInt32());

            var pubKey = message.ReadBytes(message.ReadInt32());

            //Perform the DiffieHellman protocol
            var dhKey = BigInteger.ModPow(BigInteger.ParseHex(Encoding.UTF8.GetString(pubKey)), PrivateKey, P);
            var key = dhKey.GetBytes();

            var hashService = MD5.Create();
            var s1 = Hex(hashService.ComputeHash(key, 0, key.TakeWhile(x => x != 0).Count()));
            var s2 = Hex(hashService.ComputeHash(Encoding.ASCII.GetBytes(string.Concat(s1, s1))));
            var sresult = string.Concat(s1, s2);

            var key2 = Encoding.ASCII.GetBytes(sresult);
            return key2;
        }

        private string Hex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            byte b;
            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }
            return new string(c);
        }
    }

}
