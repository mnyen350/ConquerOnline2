using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database
{
    public struct DbCastableString
    {
        public string Value;

        public DbCastableString(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(DbCastableString db)
        {
            return db.Value;
        }

        public static implicit operator byte(DbCastableString db)
        {
            return byte.Parse(db.Value);
        }

        public static implicit operator ushort(DbCastableString db)
        {
            return ushort.Parse(db.Value);
        }

        public static implicit operator uint(DbCastableString db)
        {
            return uint.Parse(db.Value);
        }

        public static implicit operator ulong(DbCastableString db)
        {
            return ulong.Parse(db.Value);
        }

        public static implicit operator sbyte(DbCastableString db)
        {
            return sbyte.Parse(db.Value);
        }

        public static implicit operator short(DbCastableString db)
        {
            return short.Parse(db.Value);
        }

        public static implicit operator int(DbCastableString db)
        {
            return int.Parse(db.Value);
        }

        public static implicit operator long(DbCastableString db)
        {
            return long.Parse(db.Value);
        }

        public static implicit operator DateTime(DbCastableString db)
        {
            return DateTime.Parse(db);
        }

        public static implicit operator TimeSpan(DbCastableString db)
        {
            return TimeSpan.Parse(db);
        }

        public static implicit operator bool(DbCastableString db)
        {
            return bool.Parse(db.Value);
        }

        public static implicit operator DbCastableString(string value)
        {
            return new DbCastableString(value);
        }

        public static implicit operator DbCastableString(byte value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(ushort value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(uint value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(ulong value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(sbyte value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(short value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(int value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(long value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(DateTime value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(TimeSpan value)
        {
            return new DbCastableString(value.ToString());
        }

        public static implicit operator DbCastableString(bool value)
        {
            return new DbCastableString(value.ToString());
        }
    }
}
