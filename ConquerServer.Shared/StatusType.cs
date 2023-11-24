using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Shared
{
    public enum StatusType
    {
        Normal = -1,
        Death = 0,
        Crime,
        Oblivion,
        TeamLeader,
        PkValue,
        Hitrate,
        Defense,
        Attack,
        Disappearing,
        MagicDefense,
        BowDefense,
        AttackRange,
        Reflect,
        Superman,
        WeaponDamage,
        MagicDamage,
        AttackSpeed,
        YinYang,
        Cyclone,
        GuildCrime,
        ReflectMagic,
        Dodge,
        Fly,       //22
        KeepBow,
        Stop,
        LuckSpread,
        LuckAbsorb,
        Curse,
        Godbless,
        RedName,
        BlackName,

        ShurikenVortex = 39,
        FatalStrike = 40,
        PoisonStar = 42,
        XPDefense = 44,

        CriticalAura = 92,
        ImmunityAura = 94,
        SoulShackle = 106,

        DetachBadly = Crime,
        DetachAll = TeamLeader,

        Internal = 250,
        Invulnerable,
        Fear,
        Hexed,
        Flag_Walk,
        Poison, //select id, name, status from game.dbo.SpellAttributes where status=2

        ExperienceModifier = 10000,
        ArenaSpectator,
        Disabled,
        Dizzy,
        Slowed,
        Frozen,
        MagicPenetration,
        InvokeShield
    }
}
