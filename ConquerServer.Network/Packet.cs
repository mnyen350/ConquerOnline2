using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public enum PacketBufferSize
    {
        /// <summary>
        /// Represents a packet buffer of 8 bytes.
        /// </summary>
        Minimum = 0,

        /// <summary>
        /// Represents a packet buffer of 16 bytes.
        /// </summary>
        SizeOf16,

        /// <summary>
        /// Represents a packet buffer of 32 bytes.
        /// </summary>
        SizeOf32,

        /// <summary>
        /// Represents a packet buffer of 64 bytes.
        /// </summary>
        SizeOf64,

        /// <summary>
        /// Represents a packet buffer of 128 bytes.
        /// </summary>
        SizeOf128,

        /// <summary>
        /// Represents a packet buffer of 256 bytes.
        /// </summary>
        SizeOf256,

        /// <summary>
        /// Represents a packet buffer of 512 bytes.
        /// </summary>
        SizeOf512,

        /// <summary>
        /// Represents a packet buffer of 1024 bytes.
        /// </summary>
        Maximum
    }

    public unsafe partial class Packet : IDisposable
    {
        public const int MaxPacketSize = 4096,
                         ExtraLargePacketSize = MaxPacketSize / 2,
                         LargePacketSize = ExtraLargePacketSize / 2,
                         MediumPacketSize = LargePacketSize / 2,
                         SmallPacketSize = MediumPacketSize / 2,
                         SmallerPacketSize = SmallPacketSize / 2,
                         SmallestPacketSize = SmallerPacketSize / 2,
                         MinPacketSize = SmallestPacketSize / 2;

        public int Size { get; protected set; }

        public int Offset { get; set; }

        public bool IsDisposed { get { return Stream == null; } }

        public byte* Stream { get; private set; }

        public IntPtr Pointer { get; private set; }

        public int BufferSize { get; private set; }

        public PacketType Type { get { return (PacketType)ReadInt16(2); } }

        private static int BufferSizeToNumber(PacketBufferSize bufferSize)
        {
            switch (bufferSize)
            {
                case PacketBufferSize.Minimum:
                    return MinPacketSize;

                case PacketBufferSize.Maximum:
                    return MaxPacketSize;

                case PacketBufferSize.SizeOf16:
                    return SmallestPacketSize;

                case PacketBufferSize.SizeOf32:
                    return SmallerPacketSize;

                case PacketBufferSize.SizeOf64:
                    return SmallPacketSize;

                case PacketBufferSize.SizeOf128:
                    return MediumPacketSize;

                case PacketBufferSize.SizeOf256:
                    return LargePacketSize;

                case PacketBufferSize.SizeOf512:
                    return ExtraLargePacketSize;
            }

            throw new NotSupportedException();
        }

        private static PacketBufferSize SizeToBufferSize(int size)
        {
            if (size <= MinPacketSize)
                return PacketBufferSize.Minimum;

            if (size <= SmallestPacketSize)
                return PacketBufferSize.SizeOf16;
            if (size <= SmallerPacketSize)
                return PacketBufferSize.SizeOf32;
            if (size <= SmallPacketSize)
                return PacketBufferSize.SizeOf64;
            if (size <= MediumPacketSize)
                return PacketBufferSize.SizeOf128;
            if (size <= LargePacketSize)
                return PacketBufferSize.SizeOf256;
            if (size <= ExtraLargePacketSize)
                return PacketBufferSize.SizeOf512;

            return PacketBufferSize.Maximum;
        }

        public Packet(PacketBufferSize bufferSize)
        {
            int size = BufferSizeToNumber(bufferSize);
            Pointer = Memory.Allocate(size);
            Stream = (byte*)Pointer;
            BufferSize = size;
            Offset = 4;

            Memory.Set(Pointer, 0, BufferSize);
        }

        public Packet(int size)
            : this(SizeToBufferSize(size))
        {
        }

        public Packet(byte[] src, int size)
            : this(size)
        {
            Memory.Copy(src, Pointer, size);
            Size = size;
        }

        public void Resize(int newSize)
        {
            if (newSize < BufferSize)
                return;

            var nPtr = Memory.Allocate(newSize);
            Memory.Set(nPtr, 0, newSize);
            Memory.Copy(Pointer, nPtr, BufferSize);
            Memory.Free(Pointer);

            Pointer = nPtr;
            Stream = (byte*)Pointer;
            BufferSize = newSize;
        }

        ~Packet()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            Memory.Free(Pointer);
            Stream = null;
            Pointer = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public void Seek(int offset)
        {
            if (Offset + offset > BufferSize || Offset + offset < 0)
                throw new Exception("Invalid seek offset.");
            Offset += offset;
        }

        public void SeekForward(int amount)
        {
            if (Offset + amount > BufferSize)
                throw new Exception("Invalid Seek amount. Value overflow.");
            Offset += amount;
        }

        public void SeekBackwards(int amount)
        {
            if (Offset - amount < 0)
                throw new Exception("Invalid Seek amount. Value underflow.");
            Offset -= amount;
        }

        public void Skip(int amount)
        {
            Offset += amount;
        }

        public void Fill(int amount)
        {
            Fill(Offset, amount);
            Offset += amount;
        }

        public void Fill(int offset, int amount)
        {
            if (offset + amount > BufferSize)
                throw new Exception("Buffer overflow.");

            for (int i = 0; i < amount; i++)
                Stream[offset + i] = 0;
        }

        #region Int64

        public void WriteDouble(double value)
        {
            WriteDouble(Offset, value);
            Offset += 8;
        }

        public void WriteDouble(int offset, double value)
        {
            if (offset + 8 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(double*)(Stream + offset) = value;
        }

        public void WriteInt64(long value)
        {
            WriteInt64(Offset, value);
            Offset += 8;
        }

        public void WriteInt64(int offset, long value)
        {
            if (offset + 8 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(long*)(Stream + offset) = value;
        }

        public void WriteUInt64(ulong value)
        {
            WriteUInt64(Offset, value);
            Offset += 8;
        }

        public void WriteUInt64(int offset, ulong value)
        {
            if (offset + 8 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(ulong*)(Stream + offset) = value;
        }

        public ulong ReadUInt64(int offset)
        {
            if (offset + 8 > Size)
                throw new Exception("Out of bounds read");
            return *(ulong*)(Stream + offset);
        }

        public ulong ReadUInt64()
        {
            var val = ReadUInt64(Offset);
            Offset += 8;
            return val;
        }

        public long ReadInt64(int offset)
        {
            if (offset + 8 > Size)
                throw new Exception("Out of bounds read");
            return *(long*)(Stream + offset);
        }

        public long ReadInt64()
        {
            var val = ReadInt64(Offset);
            Offset += 8;
            return val;
        }

        public double ReadDouble(int offset)
        {
            if (offset + 8 > Size)
                throw new Exception("Out of bounds read");
            return *(double*)(Stream + offset);
        }

        public double ReadDouble()
        {
            var val = ReadDouble(Offset);
            Offset += 8;
            return val;
        }

        #endregion Int64

        #region Int32

        public void WriteInt32Array(int[] values)
        {
            WriteInt32Array(values, values.Length);
        }

        public void WriteInt32Array(int[] values, int count)
        {
            for (int i = 0; i < count; i++)
                WriteInt32(values[i]);

            if (count > values.Length)
                Fill((count - values.Length) * 4);
        }

        public void WriteInt32(int[] values)
        {
            foreach (var value in values)
                WriteInt32(value);
        }

        public void WriteInt32(int offset, int[] values)
        {
            foreach (var value in values)
            {
                WriteInt32(offset, value);
                offset += 4;
            }
        }

        public void WriteFloat(int offset, float value)
        {
            if (offset + 4 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(float*)(Stream + offset) = value;
        }

        public void WriteFloat(float value)
        {
            WriteFloat(Offset, value);
            Offset += 4;
        }

        public void WriteInt32(int value)
        {
            WriteInt32(Offset, value);
            Offset += 4;
        }

        public void WriteInt32(int offset, int value)
        {
            if (offset + 4 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(int*)(Stream + offset) = value;
        }

        public void WriteUInt32(uint value)
        {
            WriteUInt32(Offset, value);
            Offset += 4;
        }

        public void WriteUInt32(int offset, uint value)
        {
            if (offset + 4 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(uint*)(Stream + offset) = value;
        }

        public uint ReadUInt32(int offset)
        {
            if (offset + 4 > Size)
                throw new Exception("Out of bounds read");
            return *(uint*)(Stream + offset);
        }

        public uint ReadUInt32()
        {
            var val = ReadUInt32(Offset);
            Offset += 4;
            return val;
        }

        public int ReadInt32(int offset)
        {
            if (offset + 4 > Size)
                throw new Exception("Out of bounds read");
            return *(int*)(Stream + offset);
        }

        public int[] ReadInt32Array(int offset, int count)
        {
            if (offset + 4 * count > Size)
                throw new Exception("Out of bounds read");

            var vals = new int[count];
            for (int i = 0; i < count; i++)
                vals[i] = *(int*)(Stream + (offset + count * 4));
            return vals;
        }

        public int[] ReadInt32Array(int count)
        {
            var vals = ReadInt32Array(Offset, count);
            Offset += vals.Length * 4;
            return vals;
        }

        public int[] ReadInt32Array()
        {
            var vals = ReadInt32Array(Offset, ReadInt8());
            Offset += vals.Length * 4;
            return vals;
        }

        public float ReadFloat(int offset)
        {
            if (offset + 4 > Size)
                throw new Exception("Out of bounds read");
            return *(float*)(Stream + offset);
        }

        public int ReadInt32()
        {
            var val = ReadInt32(Offset);
            Offset += 4;
            return val;
        }

        public float ReadFloat()
        {
            var val = ReadFloat(Offset);
            Offset += 4;
            return val;
        }

        #endregion Int32

        #region Int16

        public void WriteInt16Array(short[] values)
        {
            WriteInt16Array(values, values.Length);
        }

        public void WriteInt16Array(short[] values, int count)
        {
            for (int i = 0; i < count; i++)
                WriteInt16(values[i]);

            if (count > values.Length)
                Fill((count - values.Length) * 2);
        }

        public void WriteInt16(short value)
        {
            WriteInt16(Offset, value);
            Offset += 2;
        }

        public void WriteInt16(int offset, short value)
        {
            if (offset + 2 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(short*)(Stream + offset) = value;
        }

        public void WriteUInt16(ushort value)
        {
            WriteUInt16(Offset, value);
            Offset += 2;
        }

        public void WriteUInt16(int offset, ushort value)
        {
            if (offset + 2 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(ushort*)(Stream + offset) = value;
        }

        public ushort ReadUInt16(int offset)
        {
            if (offset + 2 > Size)
                throw new Exception("Out of bounds read");
            return *(ushort*)(Stream + offset);
        }

        public ushort ReadUInt16()
        {
            var val = ReadUInt16(Offset);
            Offset += 2;
            return val;
        }

        public short ReadInt16(int offset)
        {
            if (offset + 2 > Size)
                throw new Exception("Out of bounds read");
            return *(short*)(Stream + offset);
        }

        public short ReadInt16()
        {
            var val = ReadInt16(Offset);
            Offset += 2;
            return val;
        }

        public short[] ReadInt16Array(int offset, int count)
        {
            if (offset + 2 * count > Size)
                throw new Exception("Out of bounds read");

            var vals = new short[count];
            for (int i = 0; i < count; i++)
                vals[i] = *(short*)(Stream + (offset + count * 2));
            return vals;
        }

        public short[] ReadInt16Array(int count)
        {
            var vals = ReadInt16Array(Offset, count);
            Offset += vals.Length * 2;
            return vals;
        }

        public short[] ReadInt16Array()
        {
            var vals = ReadInt16Array(Offset, ReadInt8());
            Offset += vals.Length * 2;
            return vals;
        }

        #endregion Int16

        #region Int8

        public void WriteInt8(byte value)
        {
            WriteInt8(Offset, value);
            Offset += 1;
        }

        public void WriteInt8(int offset, byte value)
        {
            if (offset + 1 > BufferSize)
                throw new Exception("Buffer overflow.");
            *(Stream + offset) = value;
        }

        public byte ReadInt8(int offset)
        {
            if (offset + 1 > Size)
                throw new Exception("Out of bounds read");
            return *(Stream + offset);
        }

        public sbyte ReadSInt8()
        {
            var val = ReadInt8(Offset);
            Offset += 1;
            return (sbyte)val;
        }

        public byte ReadInt8()
        {
            var val = ReadInt8(Offset);
            Offset += 1;
            return val;
        }

        #endregion Int8

        #region Strings

        public void WriteStrings(params string[] str)
        {
            if (str.Length > 255)
                throw new Exception("Too many substrings.");

            WriteInt8((byte)str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                WriteString(str[i]);
            }
        }

        public void WriteStrings(int offset, params string[] str)
        {
            if (str.Length > 255)
                throw new Exception("Too many substrings.");

            WriteInt8(offset, (byte)str.Length);

            offset += 1;

            for (int i = 0; i < str.Length; i++)
            {
                WriteString(offset, str[i]);
                offset += 1 + str[i].Length;
            }
        }

        public void WriteString(string str)
        {
            WriteString(Offset, str);
            Offset += 1 + str.Length;
        }

        public void WriteString(int offset, string str)
        {
            if (str.Length > 255)
                throw new Exception("String is too big.");
            WriteInt8(offset, (byte)str.Length);
            var bytes = Encoding.UTF8.GetBytes(str);
            WriteBytes(offset + 1, bytes, 0, bytes.Length);
        }

        public void WriteCString(int offset, string str, int length)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var toWrite = Math.Min(bytes.Length, length);

            WriteBytes(offset, bytes, 0, toWrite);

            offset += toWrite;

            if (length - bytes.Length > 0)
                Fill(offset, length - bytes.Length);
        }

        public void WriteCString(int offset, string str)
        {
            WriteCString(offset, str, str.Length);
        }

        public void WriteCString(string str, int length)
        {
            WriteCString(Offset, str, length);
            Offset += length;
        }

        public void WriteCString(string str)
        {
            WriteCString(Offset, str, str.Length);
            Offset += str.Length;
        }

        public string ReadString()
        {
            var str = ReadString(Offset);
            Offset += str.Length + 1;
            return str;
        }

        public string[] ReadStrings()
        {
            var count = ReadInt8();
            var strings = new string[count];
            for (int i = 0; i < count; i++)
                strings[i] = ReadString();
            return strings;
        }

        public string[] ReadStrings(int count)
        {
            var strings = new string[count];
            for (int i = 0; i < count; i++)
                strings[i] = ReadString();
            return strings;
        }

        public string ReadString(int offset)
        {
            int length = ReadInt8(offset);
            offset += 1;

            var stringBytes = ReadBytes(offset, length);

            return Encoding.UTF8.GetString(stringBytes);
        }

        public string ReadCString(int offset, int length)
        {
            var bytes = ReadBytes(offset, length);

            int endOffset = bytes.Length - 1;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] == 0)
                {
                    endOffset = i;
                    break;
                }
            return Encoding.UTF8.GetString(bytes, 0, endOffset);
        }

        public string ReadCString(int length)
        {
            var bytes = ReadBytes(length);

            int endOffset = bytes.Length - 1;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] == 0)
                {
                    endOffset = i;
                    break;
                }
            return Encoding.UTF8.GetString(bytes, 0, endOffset);
        }

        #endregion Strings

        #region Byte Arrays

        public void WriteBytes(byte[] value)
        {
            WriteBytes(Offset, value, 0, value.Length);
            Offset += value.Length;
        }

        public void WriteBytes(byte[] value, int count)
        {
            if (value == null)
            {
                Fill(count);
                return;
            }

            WriteBytes(Offset, value, 0, Math.Min(value.Length, count));
            Offset += value.Length;

            if (count > value.Length)
                Fill(count - value.Length);
        }

        public void WriteBytes(int offset, byte[] value, int valueOffset, int length)
        {
            if (offset + length > BufferSize)
                throw new Exception("Buffer overflow.");
            if (length > value.Length)
                throw new Exception("Attempted to read out of value bounds");
            for (int i = 0; i < length; i++)
                Stream[offset + i] = value[valueOffset + i];
        }

        public byte[] ReadBytes()
        {
            return ReadBytes(ReadInt8());
        }

        public byte[] ReadBytes(int length)
        {
            var data = ReadBytes(Offset, length);
            Offset += length;
            return data;
        }

        public byte[] ReadBytes(int offset, int length)
        {
            if (offset + length > BufferSize)
                throw new Exception("Out of bounds read.");

            byte[] data = new byte[length];

            for (int i = 0; i < length; i++)
                data[i] = Stream[offset + i];

            return data;
        }

        #endregion Byte Arrays

        #region Unsafe

        public void WriteUnsafe(void* buf, int length)
        {
            Memory.Copy(Pointer + Offset, new IntPtr(buf), length);
            Offset += length;
        }

        public void ReadUnsafe(void* buf, int length)
        {
            Memory.Copy(new IntPtr(buf), Pointer + Offset, length);
            Offset += length;
        }

        #endregion Unsafe

        public void CopyTo(byte[] dst)
        {
            var perfSize = Size % 4;
            var left = Size - perfSize;

            fixed (byte* b = dst)
            {
                for (int i = 0; i < perfSize; i += 4)
                {
                    *(uint*)(b + i) = *(uint*)(Stream + i);
                }
            }

            if (left > 0)
            {
                for (int i = Size - left; i < Size; i++)
                    dst[i] = Stream[i];
            }
        }

        public void Build(ushort type)
        {
            Build((PacketType)type);
        }

        public virtual void Build()
        {
            Size = Offset;
        }

        public void Build(PacketType type)
        {
            WriteInt16(2, (short)type);
            WriteInt16(0, (short)Offset);
            Size = Offset;
            Offset = 4;
        }

        public static string Dump(byte* b, int len)
        {
            int msgSize = len * 4 //4 chars, 2 for number, 1 for white space, 1 for letter
                + (len / 16 + 1) * 9; //1 for /t and 2 for new line/ret, 3 for number, 2 for [ ] and 1 for space
            StringBuilder hex = new StringBuilder(msgSize);
            for (int i = 0; i < len; i += 16)
            {
                hex.AppendFormat("[{0:000}] ", i);
                for (int z = i; z < i + 16; z++)
                {
                    if (z >= len) hex.Append("   ");
                    else
                        hex.AppendFormat("{0:x2} ", b[z]);
                }
                hex.Append('\t');
                for (int z = i; z < i + 16; z++)
                {
                    if (z >= len) hex.Append(" ");
                    else
                    {
                        if (b[z] > 32 && b[z] < 127)
                        {
                            hex.AppendFormat("{0}", (char)b[z]);
                        }
                        else
                        {
                            hex.Append('.');
                        }
                    }
                }
                hex.Append("\r\n");
            }
            return hex.ToString();
        }

        public static string Dump(byte[] b)
        {
            fixed (byte* ptr = b)
                return Dump(ptr, b.Length);
        }

        public string Dump(string opt = null)
        {
            string str;
            if (string.IsNullOrEmpty(opt))
                str = "Packet - Size:[" + Size + "] Type:[" + Type + "]\r\n";
            else
                str = "Packet - Size:[" + Size + "] Type:[" + Type + "] Opt:[" + opt + "]\r\n";
            str += Dump(Stream, Size);
            return str;
        }
    }
}
