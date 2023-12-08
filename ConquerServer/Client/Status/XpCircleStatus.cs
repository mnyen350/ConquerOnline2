using ConquerServer.Network;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class XpCircleStatus : FlagStatus
    {
        public XpCircleStatus(Entity owner)
            :base(owner, StatusFlag.XPFull )
        {

        }

        public override void Attach(int power, TimeSpan? duration)
        {
            Owner.Xp = 0;
            base.Attach(power, duration);
        }
    }
}
