using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public enum LoginErrorCode
    {
        Success = 0,
        InvalidAccount = 1,
        InvalidPassword = 1,
        ServerDown = 10,
        Banned = 12,
    }
}
