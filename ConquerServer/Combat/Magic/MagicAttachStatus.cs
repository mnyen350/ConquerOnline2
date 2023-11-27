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
    public partial class Battle
    {
        [Magic(MagicSort.AttachStatus)]
        private void MagicAttachStatus()
        {
            var target = Targets.FirstOrDefault();
            if (target == null)
                return;

            // don't allow regular shiled to be cased on someone with xp shield
            if (target.Status.IsAttached(StatusType.XPDefense) && Spell?.StatusType == StatusType.Defense)
                Targets.Remove(target);
        }

        [Magic( MagicSort.DetachStatus)]
        private void MagicDetachStatus()
        {
            return;
        }
    }
}

