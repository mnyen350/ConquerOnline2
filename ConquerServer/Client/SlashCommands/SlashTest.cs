using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network.Packets;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("test")]
        private void SlashTest(string[] messageContents) 
        {
            this.Stamina = 100;
            this.Mana = this.MaxMana;
            this.SendSynchronize();            
        }
    }
}
