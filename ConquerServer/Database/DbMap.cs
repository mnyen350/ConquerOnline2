using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database
{
    public class DbMap : Dictionary<string, DbCastableString>
    {
        public DbMap Defaults { get; set; }

        public bool CanRead { get { return this.Count > 0; } }

        public DbMap()
            : base()
        {
        }

        public DbMap(int size)
            : base(size)
        {
        }

        public new DbCastableString this[string key]
        {
            get
            {
                DbCastableString result;
                if (!this.TryGetValue(key, out result))
                    throw new ArgumentException("key not found: " + key, "key");
                return result;
            }
            set
            {
                base[key] = value;
            }
        }

        public new bool TryGetValue(string key, out DbCastableString value)
        {
            if (!base.TryGetValue(key, out value))
            {
                if (this.Defaults != null)
                    return Defaults.TryGetValue(key, out value);
                return false;
            }
            return true;
        }
    }
}
