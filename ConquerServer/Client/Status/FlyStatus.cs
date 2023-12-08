using ConquerServer.Shared;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class FlyStatus : FlagStatus
    {
        public FlyStatus(Entity owner)
            :base(owner, StatusFlag.Fly)
        {

        }
    }
}
