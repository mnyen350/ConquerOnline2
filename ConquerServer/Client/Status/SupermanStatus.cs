using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class SupermanStatus : FlagStatus
    {
        public int Score { get; set; }

        public SupermanStatus(GameClient owner)
            :base(owner, StatusFlag.Superman)
        {

        }

        public override void Attach(int power, TimeSpan? duration)
        {
            base.Attach(power, duration);
        }
    }
}
