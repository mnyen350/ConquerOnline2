using ConquerServer.Client;
using ConquerServer.Database.Models;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class DetachStatusBattle : MagicBattle
    {
        protected override bool AllowDeadTarget => (Spell.Sort == MagicSort.DetachStatus && Spell.StatusType == StatusType.Death);
        public DetachStatusBattle(GameClient source, GameClient? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target, castX, castY, spell)
        {

        }

        protected override void ProcessTarget(GameClient target, Dictionary<int, (int, bool)> power)
        {

            target.Status.Detach(Spell.StatusType);
            power.Add(target.Id, (0, false));
        }
    }
}
