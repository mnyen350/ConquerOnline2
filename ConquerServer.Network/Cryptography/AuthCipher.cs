using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Cryptography
{
    public class AuthCipher : ICipher
    {
        private sealed class CryptCounter
        {
            private ushort m_Counter = 0;

            public byte Key2
            {
                get { return (byte)(m_Counter >> 8); }
            }

            public byte Key1
            {
                get { return (byte)(m_Counter & 0xFF); }
            }

            public void Increment()
            {
                m_Counter++;
            }
        }

        private CryptCounter _decryptCounter;
        private CryptCounter _encryptCounter;
        private byte[] _cryptKey1;
        private byte[] _cryptKey2;

        public AuthCipher()
        {
            _decryptCounter = new CryptCounter();
            _encryptCounter = new CryptCounter();
            _cryptKey1 = new byte[0x100];
            _cryptKey2 = new byte[0x100];
            byte i_key1 = 0x9D;
            byte i_key2 = 0x62;
            for (int i = 0; i < 0x100; i++)
            {
                _cryptKey1[i] = i_key1;
                _cryptKey2[i] = i_key2;
                i_key1 = (byte)((0x0F + (byte)(i_key1 * 0xFA)) * i_key1 + 0x13);
                i_key2 = (byte)((0x79 - (byte)(i_key2 * 0x5C)) * i_key2 + 0x6D);
            }
        }

        public byte[] EncIvec
        {
            get
            {
                return new byte[0];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public byte[] DecIvec
        {
            get
            {
                return new byte[0];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetKey(byte[] key)
        {
            throw new NotImplementedException();
        }

        public void Encrypt(byte[] src, int srcOffset, byte[] dst, int dstOffset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                dst[dstOffset + i] = (byte)(src[srcOffset + i] ^ (byte)0xAB);
                dst[dstOffset + i] = (byte)(dst[dstOffset + i] << 4 | dst[dstOffset + i] >> 4);
                dst[dstOffset + i] ^= (byte)(_cryptKey1[_encryptCounter.Key1] ^ _cryptKey2[_encryptCounter.Key2]);
                _encryptCounter.Increment();
            }
        }

        public void Decrypt(byte[] src, int srcOffset, byte[] dst, int dstOffset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                dst[dstOffset + i] = (byte)(src[srcOffset + i] ^ (byte)0xAB);
                dst[dstOffset + i] = (byte)(dst[dstOffset + i] << 4 | dst[dstOffset + i] >> 4);
                dst[dstOffset + i] ^= (byte)(_cryptKey1[_decryptCounter.Key1] ^ _cryptKey2[_decryptCounter.Key2]);
                _decryptCounter.Increment();
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
