using ConquerServer.Client;
using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class DecLifeBattle :MagicBattle
    {
        public DecLifeBattle(GameClient source, GameClient? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

        protected override void ProcessTarget(GameClient target, Dictionary<int, (int, bool)> power)
        {
            /*
             * take target current health
             * - (power-30k)%
             */
            int damage = (int)DamageAlgorithm.AdjustValue(target.Health, Spell.Power);
            target.Health -= damage;
            Source.Health += damage;
            power.Add(target.Id, (damage, false));
        }

    }
}
