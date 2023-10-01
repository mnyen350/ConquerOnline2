using System.Runtime.InteropServices;
using System.Text;

namespace ConquerServer.Database
{
    public unsafe class DbNode
    {
        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, sbyte* lpReturnedString, int nSize, string lpFileName);

        private const int
        Int32_Size = 15,
        Int16_Size = 9,
        Int8_Size = 6,
        Bool_Size = 6,
        Double_Size = 20,
        Int64_Size = 22,
        Float_Size = 10;

        public string Section { get; set; }

        public DbFile File { get; set; }

        public DbNode(DbFile file, string section)
        {
            this.File = file;
            this.Section = section;
        }

        public DbNode()
        {
        }

        public DbCastableString this[string key]
        {
            get
            {
                return new DbCastableString(ReadString(key, string.Empty));
            }
            set
            {
                WriteString(key, value);
            }
        }

        public void Create(string[] strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in strings)
            {
                sb.Append(str);
                sb.Append('\0');
            }
            sb.Append('\0');
            WritePrivateProfileSection(this.Section, sb.ToString(), this.File.Path);
        }

        public void CreateFromMap<TKey, TValue>(IDictionary<TKey, TValue> map)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in map)
            {
                sb.Append(entry.Key.ToString());
                sb.Append('=');
                sb.Append(entry.Value.ToString());
                sb.Append('\0');
            }
            sb.Append('\0');
            WritePrivateProfileSection(this.Section, sb.ToString(), this.File.Path);
        }

        public void CreateFromMap(DbMap map)
        {
            CreateFromMap<string, DbCastableString>(map);
        }

        public string ReadString(string key, string defaultString, int size = 256)
        {
            return File.ReadString(this.Section, key, defaultString, size);
        }

        public T ReadValue<T>(string key, T defaultValue, Func<string, T> parse, int size = 256)
        {
            return File.ReadValue<T>(this.Section, key, defaultValue, parse, size);
        }

        public int ReadInt32(string key, int defaultValue)
        {
            return ReadValue<int>(key, defaultValue, int.Parse, Int32_Size);
        }

        public ulong ReadUInt64(string key, ulong defaultValue)
        {
            return ReadValue<ulong>(key, defaultValue, ulong.Parse, Int64_Size);
        }

        public long ReadInt64(string key, long defaultValue)
        {
            return ReadValue<long>(key, defaultValue, long.Parse, Int64_Size);
        }

        public double ReadDouble(string key, double defaultValue)
        {
            return ReadValue<double>(key, defaultValue, double.Parse, Double_Size);
        }

        public uint ReadUInt32(string key, uint defaultValue)
        {
            return ReadValue<uint>(key, defaultValue, uint.Parse, Int32_Size);
        }

        public short ReadInt16(string key, short defaultValue)
        {
            return ReadValue<short>(key, defaultValue, short.Parse, Int16_Size);
        }

        public ushort ReadUInt16(string key, ushort defaultValue)
        {
            return ReadValue<ushort>(key, defaultValue, ushort.Parse, Int16_Size);
        }

        public sbyte ReadSByte(string key, sbyte defaultValue)
        {
            return ReadValue<sbyte>(key, defaultValue, sbyte.Parse, Int8_Size);
        }

        public byte ReadByte(string key, byte defaultValue)
        {
            return ReadValue<byte>(key, defaultValue, byte.Parse, Int8_Size);
        }

        public bool ReadBool(string key, bool defaultValue)
        {
            return ReadValue<bool>(key, defaultValue, bool.Parse, Bool_Size);
        }

        public float ReadFloat(string key, float defaultValue)
        {
            return ReadValue<float>(key, defaultValue, float.Parse, Float_Size);
        }

        public void WriteString(string key, string value)
        {
            this.File.WriteString(this.Section, key, value);
        }

        public void WriteValue<T>(string key, T value)
        {
            this.File.WriteValue<T>(this.Section, key, value);
        }

        public void Delete()
        {
            WritePrivateProfileSection(this.Section, null, this.File.Path);
        }

        public void DeleteKey(string key)
        {
            this.File.DeleteKey(this.Section, key);
        }

        public DbMap ToMap(int size = 2048)
        {
            string[] strs = ToStrings(size);
            var result = new DbMap(strs.Length);
            foreach (string str in strs)
            {
                int index = str.IndexOf('=');
                if (index > -1)
                    result.Add(str.Substring(0, index), str.Remove(0, index + 1));
            }
            return result;
        }

        private string ToStringWithNull(int size)
        {
            sbyte[] buffer = new sbyte[size];
            string str;
            fixed (sbyte* ptr = buffer)
            {
                int result = GetPrivateProfileSection(this.Section, ptr, size, this.File.Path);
                if (result <= 0)
                    return string.Empty;
                if (result == size - 2)
                    throw new ArgumentException("Insufficient buffer-space", "size");
                str = str = new string(ptr, 0, result - 1);
            }
            return str;
        }

        public string[] ToStrings(int size = 2048)
        {
            string str = ToStringWithNull(size);
            return str.Split('\0');
        }

        public string ToString(int size)
        {
            string str = ToStringWithNull(size);
            return str.Replace('\0', '\n');
        }

        public override string ToString()
        {
            return ToString(2048);
        }

        public IEnumerable<KeyValuePair<string, DbCastableString>> Where(Func<KeyValuePair<string, DbCastableString>, bool> predicate)
        {
            var matches = new List<KeyValuePair<string, DbCastableString>>();
            var query = ToMap();
            foreach (var entry in query)
                if (predicate(entry))
                    matches.Add(entry);
            return matches;
        }

        public static implicit operator DbMap(DbNode node)
        {
            return node.ToMap();
        }
    }
}
