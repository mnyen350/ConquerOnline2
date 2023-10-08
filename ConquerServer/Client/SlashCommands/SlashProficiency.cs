using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("prof")]
        private void SlashProficiency(string[] messageContents)
        {
            // COMMAND FORMAT : prof "weapon name" level(max 20) 
            // parse "weapon name" to the enum else return system failes mes
            // 0 or 20+ num will be forced 1-20 w math/min/max

            // /prof blade 20 i love tacos
            if (messageContents.Length < 3)
                throw new InvalidOperationException("Insufficient parameters");
            
            ProficiencyType weapon = (ProficiencyType)Enum.Parse(typeof(ProficiencyType), messageContents[1]);
            int level = int.Parse(messageContents[2]);

            level = Math.Max(Math.Min(level, 20),1);

            LearnProficiency(weapon, level);

        }

    }
}
