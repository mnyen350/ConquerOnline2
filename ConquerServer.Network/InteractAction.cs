using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public enum InteractAction
    {
        None = 0,
        Attack = 2,
        Court = 8,
        Marry = 9,
        Divorce = 10,
        SendFlowers = 13,
        Kill = 14,
        CheatInfo0 = 16,
        CheatInfo1 = 19,
        Unknown22 = 22,
        CheatInfo2 = 23,
        CastMagic = 24,
        AbortMagic = 25,
        Reflect = 26,
        Unknown27 = 27,
        Shoot = 28,
        ReflectMagic = 29,
        CheatInfo3 = 30,
        Unknown31 = 31,
        Unknown32 = 32,
        CheatInfo4 = 33,
        CheatInfo5 = 34,
        Unknown35 = 35,
        Unknown36 = 36,
        Unknown37 = 37,
        CheatInfo6 = 38,
        Unknown39 = 39,
        Unknown40 = 40,
        Unknown41 = 41,
        Unknown42 = 42,
        Scapegoat = 43,
        ScapegoatSwitch = 44,
        KOAttack = 45,
        Unknown46 = 46,
        InteractActAccept = 47,
        InteractActReject = 48,
        Unknown49 = 49,
        InteractActCancel = 50,
        Dismount = 51,
        Unknown52 = 52,
        MoveSkill = 53,
        Unknown54 = 54,
        Unknown55 = 55,
        Unknown56 = 56
    }
}
