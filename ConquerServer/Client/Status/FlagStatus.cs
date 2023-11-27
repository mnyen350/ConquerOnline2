using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class FlagStatus : Status
    {
        public StatusFlag Flag { get; private set; }

        public FlagStatus(GameClient owner, StatusFlag flag)
            : base(owner)
        {
            Flag = flag;
        }
        public override void Attach(int power, TimeSpan? duration)
        {
            Owner.StatusFlag += Flag;
            base.Attach(power, duration);
        }

        public override void Detach()
        {
            Owner.StatusFlag -= Flag;
            base.Detach();
        }
    }
}
