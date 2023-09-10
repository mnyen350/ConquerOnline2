using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConquerServer.Network
{
    public class TimeStamp
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime", CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetTime();

        [DllImport("msvcrt.dll", EntryPoint = "_time32", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetUnixTime32(int timer = 0);

        [DllImport("msvcrt.dll", EntryPoint = "_time64", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong GetUnixTime64(long timer = 0);
    }
}
