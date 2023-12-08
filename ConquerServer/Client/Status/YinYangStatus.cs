using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class YinYangStatus : FlagStatus
    {

        public YinYangStatus(Entity owner)
            : base(owner, StatusFlag.YinYang)
        {

        }
    }
}
