using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public class StatusFlag
    {
        public static readonly StatusFlag
            None = new StatusFlag(),
            Crime = new StatusFlag(0),
            Poison = new StatusFlag(1),
            Hidden = new StatusFlag(2),
            Freeze = new StatusFlag(3),
            XPFull = new StatusFlag(4),
            Death = new StatusFlag(5),
            TeamLeader = new StatusFlag(6),
            Hitrate = new StatusFlag(7),
            Defense = new StatusFlag(8),
            Attack = new StatusFlag(9),
            Ghost = new StatusFlag(10),
            Disappearing = new StatusFlag(11),
            MagicDefense = new StatusFlag(12),
            BowDefense = new StatusFlag(13),
            RedName = new StatusFlag(14),
            BlackName = new StatusFlag(15),
            AttackRange = new StatusFlag(16),
            Reflect = new StatusFlag(17),
            Superman = new StatusFlag(18),
            WeaponDamage = new StatusFlag(19),
            MagicDamage = new StatusFlag(20),
            AttackSpeed = new StatusFlag(21),
            Lurker = new StatusFlag(22),
            Cyclone = new StatusFlag(23),
            GuildCrime = new StatusFlag(24),
            ReflectMagic = new StatusFlag(25),
            Dodge = new StatusFlag(26),
            Fly = new StatusFlag(27),
            Intensify = new StatusFlag(28),
            Stop = new StatusFlag(29),
            LuckDiffuse = new StatusFlag(30),
            LuckAbsorb = new StatusFlag(31),
            Curse = new StatusFlag(32),
            Godbless = new StatusFlag(33),
            TopGuildLeader = new StatusFlag(34),
            TopDeputyLeader = new StatusFlag(35),
            ShurikenVortex = new StatusFlag(46),
            FatalStrike = new StatusFlag(47),
            Ride = new StatusFlag(50),
            TopSpouse = new StatusFlag(51),
            Fear = new StatusFlag(54),
            DivineShield = new StatusFlag(57),
            Stun = new StatusFlag(58),
            Frozen = new StatusFlag(59),
            Dizzy = new StatusFlag(60),
            Merchant = new StatusFlag(63),
            FlagWalk = new StatusFlag(95),
            TyrantAura = new StatusFlag(98),
            FendAura = new StatusFlag(100),
            RedAura = new StatusFlag(102),
            RedAuraAugmented = new StatusFlag(103), // boss hp
            GreenAura = new StatusFlag(104),
            BlueAura = new StatusFlag(106),
            OrangeyAura = new StatusFlag(108),
            SoulShackle = new StatusFlag(111),
            Oblivion = new StatusFlag(112),
            Flag_Jump = new StatusFlag(118),
            PurpleCloud = new StatusFlag(119),
            DefensiveStance = new StatusFlag(126),
            PurpleIcon = new StatusFlag(131),
            BlueIcon = new StatusFlag(132),
            AutoHangUp = new StatusFlag(148),
            HalfStaminaCost = new StatusFlag(159);

        private readonly BitVector _vector;

        public int[] Bits { get { return _vector.Masks; } }

        public StatusFlag(BitVector? vector = null)
        {
            _vector = vector ?? new BitVector(160);
        }

        public StatusFlag(int nFlag)
        {
            _vector = BitVector.CreateFlag(160, nFlag);
        }

        //These operators will be used to append/remove a status flag

        public static StatusFlag operator +(StatusFlag original, StatusFlag append)
        {
            return new StatusFlag(original._vector.AddFlag(append._vector));
        }

        public static StatusFlag operator -(StatusFlag original, StatusFlag remove)
        {
            return new StatusFlag(original._vector.RemoveFlag(remove._vector));
        }

        // These operators will be used to check if a StatusFlag contains another

        public static bool operator ==(StatusFlag original, StatusFlag? check)
        {
            if (check == null) return false;
            return original._vector.HasFlag(check._vector);
        }

        public static bool operator !=(StatusFlag original, StatusFlag? check)
        {
            return !(original == check);
        }

        public override bool Equals(object? obj)
        {
            // This equals function behaves different than the == operator
            // This function will check for the exact equality in terms of value of the two flags

            if (ReferenceEquals(obj, null))
                return false;

            var other = obj as StatusFlag;
            if (ReferenceEquals(other, null))
                return false;

            return this._vector.SameValue(other._vector);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            int seed = unchecked((int)0xdeadbeef);
            for (int i = 0; i < _vector.Masks.Length; i++)
            {
                hash = (_vector.Masks[i] ^ seed);
                seed = (seed >> 5) * (i + 1);
            }
            return hash;
        }

        public override string ToString()
        {
            //Implement same functionality as the [Flags] option for enumerations

            Tuple<string, StatusFlag?>[] flags =
                typeof(StatusFlag)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => Tuple.Create(f.Name, (StatusFlag?)f.GetValue(null)))
                .ToArray();

            var sb = new StringBuilder();
            for (int i = 1; i < flags.Length; i++) // exclude Zero
            {
                var entry = flags[i];
                if (this == entry.Item2)
                    sb.Append(entry.Item1 + ", ");
            }
            return (sb.Length > 0) ? sb.Remove(sb.Length - 2, 2).ToString() : string.Empty;
        }
    }
}
