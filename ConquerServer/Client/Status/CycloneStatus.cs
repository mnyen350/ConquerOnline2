using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class CycloneStatus : FlagStatus
    {
        //add to this whenever monster is killed during skill
        public int Score { get; set; }
        public CycloneStatus(GameClient owner)
            :base(owner, StatusFlag.Cyclone)
        {

        }

    }
}
