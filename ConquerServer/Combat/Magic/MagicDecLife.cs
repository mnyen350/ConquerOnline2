using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConquerServer.Combat
{
    public partial class Battle
    {
        [Magic(MagicSort.DecLife)]
        private void MagicDecLife()
        {
            return;
        }
    }
}
