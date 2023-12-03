using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("test")]
        private void SlashTest(string[] messageContents) 
        {
            var sf = new StatusFlag(int.Parse(messageContents[1]));
            if (messageContents[2] == "add")
                this.StatusFlag += sf;
            else
                this.StatusFlag -= sf;

            this.SendSynchronize();
        }

        [SlashCommand("resource")]
        private void SlashResource (string[] messageContents)
        {
            this.Stamina = 100;
            this.Mana = this.MaxMana;
            this.SendSynchronize();
        }
    }
}
