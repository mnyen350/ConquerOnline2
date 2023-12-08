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
    public class FanBattle : MagicBattle
    {
        private const int FAN_ANGLE = 45;
        public FanBattle(Entity source, Entity? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

        protected override void FindTargets()
        {
            // calculate the angle the player shot at
            int angle = Source.GetAngle(CastX, CastY);

            // calculate the lower and upper angles
            int lower = angle - FAN_ANGLE;
            int upper = angle + FAN_ANGLE;

            // find targets within the lower and upper angle
            Targets.AddRange(
                Source.FieldOfView // start with field of view
                .Where(t => (t.Distance(Source) <= Spell.Range)) // only targets in range
                .Where(t => MathHelper.IsInAngleRange(lower, Source.GetAngle(t), upper))); // only targets in angle range

        }


    }
}
