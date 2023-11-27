using ConquerServer.Client;
using ConquerServer.Shared;
using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class DodgeAlgorithm : DamageAlgorithm
    {
        private const double PLAYER_DAMAGE_REDUCTION = 0.88; //percent
        private const double PLAYER_REBORN_REDUCTION = 0.30; //percent
        public DodgeAlgorithm(GameClient source, GameClient target, MagicTypeModel? spell)
            : base(source, target, spell)
        {

        }

        public override int Calculate()
        {
            // (((RawArrowDamage)*0.12*(0.7 if other player reborn)*(1-%dodge))*(1-%tortoise)))*(1-%blessed)
            // ignore 0.12 if not player

            //rng a number in source physical attack range
            double damage = Utility.Random.Next(Source.MinPhysicalAttack, Source.MaxPhysicalAttack);

            //Increase damage with spell percentage
            damage = AdjustSpellDamage(damage);

            // if target is a player
            {
                // reduce damage by 88% -> (1-.12)
                damage *= (1 - PLAYER_DAMAGE_REDUCTION);

                // if target is reborn reduct by additional 30% -> 0.7
                if (Target.Rebirth > 0)
                {
                    damage *= (1 - PLAYER_REBORN_REDUCTION);
                }

            }

            //reduce dodge odds
            damage *= (1 - GetTargetDodgePercent());

            //reduce tortoise
            damage *= (1- GetTargetTortoiseGemPercent());

            //reduce bless
            damage *= (1 - GetTargetBlessTotal());

            return (int)Math.Max(damage,1);
        }
    }
}
