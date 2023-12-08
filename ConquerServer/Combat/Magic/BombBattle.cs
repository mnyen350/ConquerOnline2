using ConquerServer.Network;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Database.Models;
using ConquerServer.Client;

namespace ConquerServer.Combat
{
    public class BombBattle : MagicBattle
    {
        public BombBattle(Entity source, Entity? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }
        protected override void FindTargets()
        {
            // find all targets in range
            // TO-DO: circle
            Targets.AddRange(Source.FieldOfView.Where(p => p.Distance(Source) <= Spell.Range));
        }
    }
}
