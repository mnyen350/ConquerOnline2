using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class DefenseStatus : FlagStatus
    {
        public DefenseStatus(GameClient owner)
            :base(owner, StatusFlag.Defense)
        {

        }

        public override void Attach(int power, TimeSpan? duration)
        {
            Owner.Status.Detach(StatusType.Defense);
            Owner.Status.Detach(StatusType.XPDefense);
            base.Attach(power, duration);
        }
    }
}
