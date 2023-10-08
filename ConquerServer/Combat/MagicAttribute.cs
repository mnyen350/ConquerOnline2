using ConquerServer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Database;
using ConquerServer.Shared;

namespace ConquerServer.Combat
{
    public class MagicAttribute : Attribute, IDispatcherAttribute<MagicSort>
    {
        public MagicSort[] Keys { get; private set; }

        public MagicAttribute(params MagicSort[] sorts)
        {
            Keys = sorts;
        }
    }
}
