using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    internal class HitRateStatus : FlagStatus
    {
        public HitRateStatus(Entity owner)
            : base(owner, StatusFlag.Hitrate)
        {

        }

    }
}
