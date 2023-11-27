using ConquerServer.Client;
using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class MagicAlgorithm : DamageAlgorithm
    {
        public MagicAlgorithm(GameClient source, GameClient target, MagicTypeModel? spell)
            : base(source, target, spell)
        {

        }

        public override int Calculate()
        {
            // damage is the MagicAttack of the caster
            double damage = Source.MagicAttack;

            // add on the spell power (it's flat)
            damage = AdjustSpellDamage(Spell?.Power ?? 0);


            // increase based off of phoenix gems
            damage *= GetPhoenixGemPercent();

            // crit
            if (IsCriticalHit())
                damage *= 1.5;

            // reduce based on target's Magic Defense (it's a %)
            damage *= 1.00 - Target.MagicDefense * 0.01;

            //Decrease damage with Bless percentage(From Items)
            damage *= 1.00 - GetTargetBlessTotal();

            //Decrease damage with Tortoise percentage(From Gems)
            damage *= 1.00 - GetTargetTortoiseGemPercent();

            //Damage + Fan Final Damage
            damage += GetFanFinalMagic();

            //Damage - Tower Final Damage
            damage -= GetTargetTowerMagicDefense();

            //If Lucky Chance blocks attack, damage = 1
            if (IsTargetLucky())
                damage = 1;

            //If Block activates Damage *0.5
            if (IsTargetBlock())
                damage *= 0.5;

            // return final result
            return Math.Max((int)damage, 1);
        }
    }
}
