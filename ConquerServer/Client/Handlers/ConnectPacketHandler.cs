using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [Network(PacketType.Connect)]
        private async Task ConnectPacketHandler(Packet p)
        {
            int accountId = p.ReadInt32();
            int key = p.ReadInt32();
            int version = p.ReadInt16();
            string region = p.ReadCString(2);
            long mac = p.ReadInt64();
            int version2 = p.ReadInt32();

            AuthClient? auth;
            if (!Database.Auth.Remove(accountId, out auth) ||
                auth.Username == null ||
                auth.Server == null) // invalid
            {
                Disconnect();
                return;
            }

            Username = auth.Username;
            Server = auth.Server;
#warning server aspect removed for simplicity
            if (!Database.HasCharacter())
            {
                // if no characterfile found.. 
                // send to character creation
                SendChat(ChatMode.Entrance, "NEW_ROLE");
#warning send user/player to char select
            }
            else
            {
                // continue the login process
                await StartLoginSequence();
            }
        }
    }
}
