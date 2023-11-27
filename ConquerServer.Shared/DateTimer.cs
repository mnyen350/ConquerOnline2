using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Shared
{
    public class DateTimer
    {
        public DateTime ReadyAt { get; private set; }
        public bool IsReady => DateTime.UtcNow >= ReadyAt;
        public DateTimer()
        {
            ReadyAt = DateTime.UtcNow;
        }

        public void Set(TimeSpan delay)
        {
            ReadyAt = DateTime.UtcNow.Add(delay);
        }

        //public static implicit operator bool(DateTimer dt) => dt.IsReady;
    }
}
