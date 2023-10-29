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
        [Magic(MagicSort.AttackStatus, MagicSort.Attack)]
        private void MagicAttackStatus()
        {
            // Targets already contains the target being hit
            return;
        }
    }
}
