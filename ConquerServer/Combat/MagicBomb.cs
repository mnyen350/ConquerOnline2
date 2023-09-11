using ConquerServer.Network;
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
        [Magic(MagicSort.Bomb)]
        private void MagicBomb()
        {
            // determine x,y of center of "bomb"
            int x = Source.X;
            int y = Source.Y;
            // find range
            int range = Spell.Range;
            // find all targets in range
            // TO-DO: circle
            Targets.AddRange(Source.FieldOfView.Where(p => p.Distance(Source) <= range));
        }
    }
}
