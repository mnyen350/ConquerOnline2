using ConquerServer.Shared;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class FlyStatus : Status
    {
        public FlyStatus(GameClient owner)
            :base(owner)
        {

        }

        public override void Attach(int power, TimeSpan? duration)
        {
            Owner.StatusFlag += StatusFlag.Fly;

            Owner.SendSynchronize(true);

            base.Attach(power, duration);
            
        }

        public override void Detach()
        {
            Owner.StatusFlag -= StatusFlag.Fly;

            Owner.SendSynchronize(true);

            base.Detach();
        }
    }
}
