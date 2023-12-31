﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("teleport", "tp")]
        private void SlashTeleport(string[] messageContents)
        {
            // teleport as requested (/teleport 1002 400 400)
#warning assuming all given mapIds are valid, sanity check later

            int[] ints = new int[] { this.MapId, this.X, this.Y };

            //teleporting
            this.Teleport(int.Parse(messageContents[1]), int.Parse(messageContents[2]), int.Parse(messageContents[3]));
        }
    }
}
