using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public class BitVector
    {
        public int[] Masks { get; private set; }

        public int BitCount { get { return Masks.Length * 32; } }

        public BitVector(int bits)
        {
            if (bits <= 0)
                throw new ArgumentException("bits<=0");
            int nMask = ((bits - 1) / 32) + 1;
            Masks = new int[nMask];
        }

        public bool this[int bitIndex]
        {
            get
            {
                int sector = bitIndex / 32;
                int mask = Masks[sector];
                int flag = 1 << (bitIndex - (sector * 32));
                return (mask & flag) == flag;
            }
            set
            {
                int sector = bitIndex / 32;
                int flag = 1 << (bitIndex - (sector * 32));
                if (value)
                    Masks[sector] |= flag;
                else
                    Masks[sector] &= ~flag;
            }
        }

        private static BitVector AddFlag(BitVector little, BitVector big)
        {
            var res = new BitVector(big.BitCount);
            for (int i = 0; i < big.Masks.Length; i++)
            {
                if (i < little.Masks.Length)
                    res.Masks[i] = little.Masks[i] | big.Masks[i];
                else
                    res.Masks[i] = big.Masks[i];
            }
            return res;
        }

        public BitVector AddFlag(BitVector other)
        {
            // flag |= other
            if (other.Masks.Length > this.Masks.Length)
                return AddFlag(this, other);
            return AddFlag(other, this);
        }

        public BitVector RemoveFlag(BitVector other)
        {
            // flag &= ~other
            int min = Math.Min(other.Masks.Length, this.Masks.Length);
            var res = new BitVector(this.BitCount);
            for (int i = 0; i < min; i++)
                res.Masks[i] = this.Masks[i] & (~other.Masks[i]);
            return res;
        }

        public bool HasFlag(BitVector other)
        {
            if (other.Masks.Length > Masks.Length)
            {
                for (int i = Masks.Length; i < other.Masks.Length; i++)
                    if (other.Masks[i] > 0)
                        return false;
            }
            for (int i = 0; i < Masks.Length; i++)
            {
                if ((Masks[i] & other.Masks[i]) != other.Masks[i])
                    return false;
            }
            return true;
        }

        private static bool SameValue(BitVector little, BitVector big)
        {
            for (int i = little.Masks.Length; i < big.Masks.Length; i++)
                if (big.Masks[i] != 0)
                    return false;
            for (int i = 0; i < little.Masks.Length; i++)
                if (big.Masks[i] != little.Masks[i])
                    return false;
            return true;
        }

        public bool SameValue(BitVector other)
        {
            if (other.Masks.Length == this.Masks.Length)
            {
                for (int i = 0; i < this.Masks.Length; i++)
                    if (other.Masks[i] != this.Masks[i])
                        return false;
                return true;
            }
            else if (other.Masks.Length > this.Masks.Length)
                return SameValue(this, other);
            return SameValue(other, this);
        }

        public static BitVector CreateFlag(int maxBits, int flagIndex)
        {
            var resultant = new BitVector(maxBits);
            resultant[flagIndex] = true;
            return resultant;
        }
    }
}
