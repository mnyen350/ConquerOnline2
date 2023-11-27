using ConquerServer.Client;
using ConquerServer.Database.Models;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public abstract class DamageAlgorithm
    {
        public GameClient Source { get; private set; }
        public GameClient Target { get; private set; }
        public MagicTypeModel? Spell { get; private set; }

        public DamageAlgorithm(GameClient source, GameClient target, MagicTypeModel? spell)
        {
            Source = source;
            Target = target;
            Spell = spell;
        }

        protected double AdjustValue(double value, int adjust)
        {
            if (adjust >= 30000)
            {
                adjust -= 30000;
                return value * adjust / 100;
            }
            return value + adjust;
        }

        protected double GetTargetDodgePercent()
        {
            //total/sum the dodge of all items the target has equiped

            //loop target gear, add all dodge values
            return ((double)Target.Equipment.Dodge / 100); //.75

        }

        protected double GetDragonGemPercent()
        {
            // Normal = 5%
            // Refined = 10%
            // Super = 15%

            return 1.00;
        }
        
        protected double GetPhoenixGemPercent()
        {
            // Normal = 5%
            // Refined = 10%
            // Super = 15%

            return 1.00;
        }

        protected double AdjustDefense(double damage)
        {
            //return value is the damage dealt to target

            //flat defense == defnese+power 
            //... damage - defnes 

            //percent -30k/100 -> perceot 
            //defenes*percent

            double defense = AdjustValue(Target.PhysicalDefense, Target.Status.GetPower(StatusType.Defense));
            defense = AdjustValue(defense, Source.Status.GetPower(StatusType.XPDefense));
        
            return damage - defense;
        }

        protected double AdjustAttackStatus(double damage)
        {
            // if power is > 30000, it's a percent
            // the way we get the % is (power-30000)/100
            // if its not (so under or eq to 30000)
            // it's flat value

            damage = AdjustValue(damage, Source.Status.GetPower(StatusType.Attack));
            damage = AdjustValue(damage, Source.Status.GetPower(StatusType.Superman));

            //superman - warrior 
            if (Source.Status.IsAttached(StatusType.Superman) && true /* TO-DO: is player */)
                damage /= 5; // only 2x against players

            return damage;
        }

        protected bool IsCriticalHit()
        {
            return false;
        }

        protected double GetTargetShieldDefense()
        {
            return 0.00; //represents % reduced so like .3
        }

        protected double GetTargetBlessTotal()
        {
            //sum the blessing
            //total is amount reduced by like total 22 = return .22
            return 0.00;
        }

        protected double GetTargetTortoiseGemPercent()
        {
            // Normal = 5%
            // Refined = 10%
            // Super = 15%

            //total is amount reduced by like total 40% = return .4

            return 0.00;
        }

        protected double GetFanFinalPhysical()
        {
            return 0;
        }

        protected double GetFanFinalMagic()
        {
            return 0;
        }

        protected double GetTargetTowerPhysicalDefense()
        {
            return 0;
        }

        protected double GetTargetTowerMagicDefense()
        {
            return 0;
        }

        protected bool IsTargetLucky()
        {
            return false;
        }

        protected bool IsTargetBlock()
        {
            return false;
        }

        protected double AdjustSpellDamage(double damage)
        {
            damage = AdjustValue(damage, Spell?.Power ?? 0);

            return damage;
        }

        public abstract int Calculate();
    }
}