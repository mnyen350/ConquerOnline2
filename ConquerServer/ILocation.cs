using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public interface ILocation
    {
        public int X { get; }
        public int Y { get; }
        public int MapId { get; }
    }
}
