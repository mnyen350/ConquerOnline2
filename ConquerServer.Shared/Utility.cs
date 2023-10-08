using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;

namespace ConquerServer.Shared
{
    public static class Utility
    {
        public static Random Random { get; private set; }

        static Utility()
        {
            Random = new Random();
        }

        public static void Delay(TimeSpan when, Func<Task> task)
        {
            Task.Run(async () =>
            {
                await Task.Delay(when);
                await task();
            });
        }

        public static void Delay(TimeSpan when, Action callback)
        {
            Delay(when, () => new Task(callback));
        }

        public static bool IsDefined<T>(this T enumValue)
            where T : Enum
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
        {
            foreach (var item in range)
                hashSet.Add(item);
        }

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
