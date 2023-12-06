using ConquerServer.Client;
using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class RecruitBattle : MagicBattle
    {
        public RecruitBattle(GameClient source, GameClient? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

        protected override void ProcessTarget(GameClient target, Dictionary<int, (int, bool)> power)
        {
            if (Spell == null)
                return;

            target.Health += Spell.Power;
            power.Add(target.Id, (Spell.Power, false));
        }

    }
}
