using ConquerServer.Client;
using ConquerServer.Database.Models;
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

        protected double GetStigmaPercent()
        {
            return 1.00;
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

        protected double GetSpellDamagePercent()
        {
            if (Spell != null && Spell.Power > 30000)
            {
                double percent = Spell.Power - 30000;
                return percent / 100;
            }
            return 1.00;
        }

        public abstract int Calculate();
    }
}