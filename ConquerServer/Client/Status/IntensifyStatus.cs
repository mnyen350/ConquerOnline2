using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class IntensifyStatus : FlagStatus
    {
        public IntensifyStatus(Entity owner)
            :base(owner, StatusFlag.Intensify)
        {

        }
    }
}
