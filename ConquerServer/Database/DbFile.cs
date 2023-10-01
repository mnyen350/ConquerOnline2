using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConquerServer.Database
{
    public unsafe class DbFile
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, sbyte* lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSectionNames(sbyte* lpszReturnBuffer, int nSize, string lpFileName);

        private const int
        Int32_Size = 15,
        Int16_Size = 9,
        Int8_Size = 6,
        Bool_Size = 6,
        Double_Size = 20,
        Int64_Size = 22,
        Float_Size = 10;

        public string Path { get; set; }

        public bool Exists { get { return File.Exists(Path); } }

        public DbFile()
        {
        }

        public DbFile(string fileName)
        {
            this.Path = fileName;
        }

        public DbNode this[string section]
        {
            get
            {
                return new DbNode(this, section);
            }
            set
            {
                DbNode node = new DbNode(this, section);
                node.Create(value.ToStrings());
            }
        }

        public DbCastableString this[string section, string key]
        {
            get
            {
                return new DbCastableString(ReadString(section, key, string.Empty));
            }
            set
            {
                WriteString(section, key, value);
            }
        }

        public void DeleteNode(string section)
        {
            DbNode node = new DbNode(this, section);
            node.Delete();
        }

        public DbNode[] GetNodes(int size = 2048)
        {
            sbyte[] buffer = new sbyte[size];
            string str;
            fixed (sbyte* ptr = buffer)
            {
                int amount = GetPrivateProfileSectionNames(ptr, size, this.Path);
                if (amount == size - 2)
                    throw new ArgumentException("Insufficient buffer-space", "size");
                if (amount <= 0)
                    return new DbNode[0];
                str = new string(ptr, 0, amount - 1);
            }
            string[] nodeNames = str.Split('\0');
            DbNode[] nodes = new DbNode[nodeNames.Length];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = new DbNode(this, nodeNames[i]);
            return nodes;
        }

        public string ReadString(string section, string key, string defaultString, int size = 256)
        {
            sbyte* buffer = stackalloc sbyte[size];
            GetPrivateProfileString(section, key, defaultString, buffer, size, this.Path);
            return new string(buffer).Trim('\0');
        }

        public T ReadValue<T>(string section, string key, T defaultValue, Func<string, T> parse, int size = 256)
        {
            try
            {
                return parse(ReadString(section, key, defaultValue.ToString(), size));
            }
            catch
            {
                return defaultValue;
            }
        }

        public int ReadInt32(string section, string key, int defaultValue)
        {
            return ReadValue<int>(section, key, defaultValue, int.Parse, Int32_Size);
        }

        public ulong ReadUInt64(string section, string key, ulong defaultValue)
        {
            return ReadValue<ulong>(section, key, defaultValue, ulong.Parse, Int64_Size);
        }

        public long ReadInt64(string section, string key, long defaultValue)
        {
            return ReadValue<long>(section, key, defaultValue, long.Parse, Int64_Size);
        }

        public double ReadDouble(string section, string key, double defaultValue)
        {
            return ReadValue<double>(section, key, defaultValue, double.Parse, Double_Size);
        }

        public uint ReadUInt32(string section, string key, uint defaultValue)
        {
            return ReadValue<uint>(section, key, defaultValue, uint.Parse, Int32_Size);
        }

        public short ReadInt16(string section, string key, short defaultValue)
        {
            return ReadValue<short>(section, key, defaultValue, short.Parse, Int16_Size);
        }

        public ushort ReadUInt16(string section, string key, ushort defaultValue)
        {
            return ReadValue<ushort>(section, key, defaultValue, ushort.Parse, Int16_Size);
        }

        public sbyte ReadSByte(string section, string key, sbyte defaultValue)
        {
            return ReadValue<sbyte>(section, key, defaultValue, sbyte.Parse, Int8_Size);
        }

        public byte ReadByte(string section, string key, byte defaultValue)
        {
            return ReadValue<byte>(section, key, defaultValue, byte.Parse, Int8_Size);
        }

        public bool ReadBool(string section, string key, bool defaultValue)
        {
            return ReadValue<bool>(section, key, defaultValue, bool.Parse, Bool_Size);
        }

        public float ReadFloat(string section, string key, float defaultValue)
        {
            return ReadValue<float>(section, key, defaultValue, float.Parse, Float_Size);
        }

        public void Delete(string section, string key)
        {
            DbNode node = new DbNode(this, section);
            node.Delete();
        }

        public void DeleteKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, this.Path);
        }

        public void WriteString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.Path);
        }

        public void WriteValue<T>(string section, string key, T value)
        {
            WriteString(section, key, value.ToString());
        }

        public IEnumerable<DbNode> Where(Func<DbNode, bool> predicate)
        {
            var nodes = new List<DbNode>();
            foreach (DbNode node in this.GetNodes())
            {
                if (predicate(node))
                    nodes.Add(node);
            }
            return nodes;
        }

        public IEnumerable<TResult> Select<TResult>(Func<DbNode, TResult> selector)
        {
            var nodes = new List<TResult>();
            foreach (DbNode node in this.GetNodes())
                nodes.Add(selector(node));
            return nodes;
        }
    }
}
