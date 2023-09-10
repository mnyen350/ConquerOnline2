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
        [SlashCommand("test")]
        private void SlashTest(string[] messageContents) 
        {
            using (var p = new SynchronizePacket()
                                    .Begin(this.Id)
                                    .Synchronize(SynchronizeType.Stamina, 100)
                                    .End())
            {
                this.Send(p);
            }
        }
    }
}
