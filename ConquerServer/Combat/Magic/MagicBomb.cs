using ConquerServer.Network;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Database.Models;

namespace ConquerServer.Combat
{
    public partial class Battle
    {
        [Magic(MagicSort.Bomb, MagicSort.BombGroundTarget)]
        private void MagicBomb()
        {
            // find all targets in range
            // TO-DO: circle
            Targets.AddRange(Source.FieldOfView.Where(p => p.Distance(Source) <= Spell.Range));
        }
    }
}
