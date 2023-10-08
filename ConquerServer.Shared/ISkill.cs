using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Shared
{
    public interface ISkill
    {
        long Experience { get; }
        int TypeId { get;  }
        int Level { get; }
    }
}
