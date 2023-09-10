using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("item")]
        private void SlashItem(string[] messageContents)
        {
            int typeId = int.Parse(messageContents[1]);

            Item test = Database.CreateItem(typeId);
            this.Inventory.TryAdd(test);
        }
    }
}
