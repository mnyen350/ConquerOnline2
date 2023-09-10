using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConquerServer.Network
{
    public class Memory
    {
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr Set(IntPtr dest, int value, int count);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr Copy(IntPtr dest, IntPtr src, int count);

        public static void Copy(byte[] src, IntPtr dest, int count)
        {
            Marshal.Copy(src, 0, dest, count);
        }

        public static void Copy(IntPtr src, byte[] dst, int count)
        {
            Marshal.Copy(src, dst, 0, count);
        }

        public static IntPtr Allocate(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        public static void Free(IntPtr pBuffer)
        {
            Marshal.FreeHGlobal(pBuffer);
        }
    }
}
