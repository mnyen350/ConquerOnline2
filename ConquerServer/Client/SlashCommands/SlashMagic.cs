using ConquerServer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("magic")]
        private void SlashMagic(string[] messageContents)
        {
            // typeid + level
            int typeId = int.Parse(messageContents[1]);
            int level = int.Parse(messageContents[2]);
            // create method to create magic... to call here
            // method handles : add to this.magics + packetsending 
            if (Db.GetMagicType(typeId, level) != null)
            {
                LearnMagic(typeId, level);
            }
        }
    }
}
