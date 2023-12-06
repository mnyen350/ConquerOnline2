using ConquerServer.Client;
using ConquerServer.Combat;
using ConquerServer.Database.Models;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConquerServer.Combat
{
    public class AttachStatusBattle : MagicBattle
    {
        public AttachStatusBattle(GameClient source, GameClient? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

        protected override void ProcessTarget(GameClient target, Dictionary<int, (int, bool)> power)
        {
            target.Status.Attach(Spell.StatusType, Spell.Power, TimeSpan.FromSeconds(Spell.StepSecond));
            power.Add(target.Id, (Spell.StepSecond, false));
        }

        protected override PotentialTargetError IsPotentialTarget(GameClient target)
        {
            // cannot cast Defense(magic shield) status onto a target with XPDefense(shield)
            if (Spell.StatusType == StatusType.Defense && target.Status.IsAttached(StatusType.XPDefense))
                return PotentialTargetError.XPDefense;

            return base.IsPotentialTarget(target);
        }
    }
}

