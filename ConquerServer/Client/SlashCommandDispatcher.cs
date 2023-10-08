using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;

namespace ConquerServer.Client
{
    public class SlashCommandDispatcher
        :Dispatcher<string, SlashCommandAttribute, Action<GameClient, string[]>>
    {
        public SlashCommandDispatcher() 
            :base(typeof(GameClient))
        {
            
        }
    }
}
