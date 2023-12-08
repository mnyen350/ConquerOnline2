using ConquerServer.Client;
using ConquerServer.Shared;
using ConquerServer.Database.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ConquerServer.Combat
{
    public class PhysicalAlgorithm : DamageAlgorithm
    {
  

        public PhysicalAlgorithm(Entity source, Entity target, MagicTypeModel? spell)
            : base(source, target, spell)
        {
        }   

       
        public override int Calculate()
        {
            //Get random value between MinAttack and MaxAttack
            double damage = Utility.Random.Next(Source.MinPhysicalAttack, Source.MaxPhysicalAttack);

            //Increase damage with spell percentage
            damage = AdjustSpellDamage(damage);

            //Increase damage with DragonGems percentage
            damage *= GetDragonGemPercent();

            //Increase damage with Stigma percentage
            damage = AdjustAttackStatus(damage);

            //Get Break Chance, if false reduce by 50 %
            //---

            //Get CriticalChance and increase 1.5x
            if (IsCriticalHit())
                damage *= 1.5;

            //Get Heaven Blessing 2x Attack Chance
            //---

            //Subtract damage with defense
            damage = AdjustDefense(damage);

            //Decrease damage with Magic / Shield defense
            damage *= (1.00 - GetTargetShieldDefense());

            //Decrease damage with Bless percentage(From Items)
            damage *= (1.00 - GetTargetBlessTotal());

            //Decrease damage with Tortoise percentage(From Gems)
            damage *= (1.00 - GetTargetTortoiseGemPercent());

            //Decrease damage with reborn reduct
            //---

            //Decrease damage from Azure Shield
            //---

            //Damage + Fan Final Damage
            damage += GetFanFinalPhysical();

            //Damage - Tower Final Damage
            damage -= GetTargetTowerPhysicalDefense();

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
