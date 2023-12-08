using ConquerServer.Client;
using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class AddManaBattle : MagicBattle
    {
        public AddManaBattle(Entity source, Entity? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

        protected override void ProcessTarget(Entity target, Dictionary<int, (int, bool)> power)
        {
            target.Mana += Spell.Power;
            power.Add(target.Id, (Spell.Power, false));
        }
    }
}
