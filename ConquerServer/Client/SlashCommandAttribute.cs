using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class SlashCommandAttribute : Attribute, IDispatcherAttribute<string>
    {
        public string[] Keys { get; private set; }
        public SlashCommandAttribute(params string[] commands)
        {
            Keys = commands;
        }
    }
}
