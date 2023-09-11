using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public class MagicDispatcher
        :Dispatcher<MagicSort, MagicAttribute, Action<Battle>>
    {
        public MagicDispatcher() 
            :base(typeof(Battle))
        {

        }
    }
}
