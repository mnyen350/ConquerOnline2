using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network;
using ConquerServer.Network.Sockets;
using ConquerServer.Database;

namespace ConquerServer.Client
{
    public class AuthClient
    {
        private static int g_IdCounter;
        private static NetworkDispatcher<AuthClient> g_Network;
        static AuthClient()
        {
            g_IdCounter = 1000;
            g_Network = new NetworkDispatcher<AuthClient>();
        }

        public ClientSocket Socket { get; private set; }
        public int Id { get; private set; }
        public string? Username { get; private set; }
        public string? Server { get; private set; }
        public DateTime AuthenticatedAt { get; private set; }
        public Db Database { get; private set; }

        public AuthClient(ClientSocket socket)
        {
            Id = g_IdCounter++;
            Socket = socket;
            Database = new Db(null);
        }

        public void ReplyToLogin(LoginErrorCode code)
        {
            ReplyToLogin(0, (int)code, "", 0);
        }

        public void ReplyToLogin(int accountId, int sessionId, string ip, int port)
        {
            using (var p = new Packet(PacketBufferSize.SizeOf64))
            {
                p.WriteInt32(accountId);
                p.WriteInt32(sessionId);
                p.WriteInt32(port);
                p.WriteInt32(0);
                p.WriteCString(ip, 16);
                p.Build(PacketType.ConnectEx);

                Socket.Send(p);
            }
        }

        public void Process(Packet p)
        {
            var action = g_Network[p.Type];
            action?.Invoke(this, p);
        }

        [Network(PacketType.AccountSrp6Ex)]
        public void AccountSrp6Ex(Packet p)
        {
            p.ReadInt32();
            string username = p.ReadCString(16);
            p.ReadBytes(112);
            string server = p.ReadCString(16);
            string mac = p.ReadCString(16);

            Username = username;
            Server = server;
            AuthenticatedAt = DateTime.UtcNow;

            // TO-DO: if after a certamin amount of time, i.e. 5 minutes
            // remove the client from World.AuthClients as we can anticipate
            // that it will never be used

            Database.Auth.Add(Id, this);
            ReplyToLogin(Id, Id, Database.ServerHost, 5816);

            Console.WriteLine("Redirected auth client to game server...");
        }
    }
}
