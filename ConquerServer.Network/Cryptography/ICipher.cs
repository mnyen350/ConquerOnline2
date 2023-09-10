using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Cryptography
{
    public interface ICipher
    {
        byte[] EncIvec { get; set; }

        byte[] DecIvec { get; set; }

        void SetKey(byte[] key);

        void Encrypt(byte[] src, int srcOffset, byte[] dst, int dstOffset, int length);

        void Decrypt(byte[] src, int srcOffset, byte[] dst, int dstOffset, int length);

        void Reset();
    }
}
