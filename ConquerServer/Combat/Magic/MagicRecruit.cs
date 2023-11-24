using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public partial class Battle
    {
        [Magic(MagicSort.Recruit)]
        private void MagicRecruit()
        {
            return; 
        }
    }
}
