using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public static class Utility
    {
        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue? dummy;
            return dictionary.TryRemove(key, out dummy);
        }

        public static int Distance(this ILocation l1, ILocation l2)
        {
            if (l1.MapId != l2.MapId)
                return int.MaxValue;
            return MathHelper.GetDistance(l1.X, l2.Y, l2.X, l2.Y);
        }
    }
}
