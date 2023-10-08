using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Packets
{
    //
    // Once again, big thanks to ntl3fty for the up to date list!
    //

    public enum SynchronizeType
    {
        None = -1,
        Health = 0,
        MaxLife = 1,
        Mana = 2,
        MaxMana = 3,
        Money = 4,
        Experience = 5,
        PKPoints = 6,
        Profession = 7,
        Stamina = 8,
        MoneySaved = 9,
        AdditionalPoint = 10,
        Lookface = 11,
        Level = 12,
        Spirit = 13,
        Vitality = 14,
        Strength = 15,
        Agility = 16,
        HeavenBlessing = 17,
        DoubleExpTime = 18,
        CursedTime = 20,
        SupermanOrder = 21,
        Metempsychosis = 22,
        Virtue = 23,
        Flags = 25,
        Hair = 26,
        XPCircle = 27,
        LuckyTime = 28,
        EMoney = 29,
        XpTime = 30,
        OnlineTraining = 31,
        Enthrallment1 = 32,
        Enthrallment2 = 33,
        Enthrallment3 = 34,
        Enthrallment4 = 35,
        MentorBattlePower = 36,
        MetempsychosisLevel = 37,
        Merchant = 38,
        Vip = 39,
        QuizPoint = 40,
        EnlightenPoint = 41,
        Unknown42 = 42,
        DoublePkPartnerLife = 43,
        GuildBattlePower = 44,
        BoundEMoney = 45,
        RacePoint = 47,
        ClanBattlePower = 48,
        AzureShield = 49, // more than just azureshield
        FirstRebirthProfession = 50,
        BirthProfession = 51,
        TeamId = 52,
        SoulShackle = 54,
        Fatigue = 55,
        DefensiveStance = 56,
        CampBattleLevel = 57,
        CampMaskNum = 58,
        AddPhysicalCriticalRate = 59,
        AddMagicCriticalRate = 60,
        AddAntiCriticalRate = 61,
        AddSmashAttackRate = 62,
        AddFirmDefenseRate = 63,
        AddMaxLife = 64,
        AddPhysicalAttack = 65,
        AddMagicAttack = 66,
        AddPhysicalDamageReduce = 67,
        AddMagicDamageReduce = 68,
        AddFinalPhysicalDamage = 69,
        AddFinalMagicDamage = 70,
        SetPrivilege = 71,
        ExpProtection = 73,

        // custom
        ChangeSize = 255, // sizeadd, zoom percent

        MoveSpeed = 256, // move speed
        RoleType = 257, // interact type...

        GuildMoney = 1000,
    }

    public class SynchronizePacket : Packet
    {
        public SynchronizePacket()
        : base(512)
        {
        }
        public unsafe void IncrementSyncCount()
        {
            *(int*)&Stream[12] += 1;
        }

        public SynchronizePacket Begin(int playerId)
        {
            Offset = 4;
            WriteUInt32(TimeStamp.GetTime());   // 4
            WriteInt32(playerId);               // 8
            WriteInt32(0);                      // 12 (count)
            return this;
        }

        /*public SynchronizePacket Synchronize(StatusFlag flag)
        {
            IncrementSyncCount();
            this.WriteUInt32((uint)SynchronizeType.Flags);
            foreach (var bits in flag.Bits)
                this.WriteInt32(bits);
            return this;
        }*/

        public SynchronizePacket Synchronize(SynchronizeType type, uint value, uint value2 = 0)
        {
            IncrementSyncCount();
            WriteUInt32((uint)type);
            WriteUInt64((ulong)value2 << 32 | value);
            WriteUInt64(0ul);
            WriteUInt32(0u);
            return this;
        }

        public SynchronizePacket Synchronize(SynchronizeType type, int[] values)
        {
            if (values.Length != 5)
                throw new Exception("Error: Synchronize Packet - The sycnrhonize value size is incorrect (requires 5 ints)");

            IncrementSyncCount();
            WriteUInt32((uint)type);
            WriteInt32Array(values);
            return this;
        }

        public SynchronizePacket Synchronize(SynchronizeType type, ulong value)
        {
            IncrementSyncCount();
            WriteUInt32((uint)type);
            WriteUInt64(value);
            WriteUInt64(0ul);
            WriteUInt32(0u);
            return this;
        }

        public SynchronizePacket Synchronize(SynchronizeType type, int value, int value2 = 0)
        {
            return Synchronize(type, (uint)value, (uint)value2);
        }

        public SynchronizePacket Synchronize(SynchronizeType type, long value)
        {
            return Synchronize(type, (ulong)value);
        }

        public SynchronizePacket End()
        {
            Build(PacketType.UserAttribute);
            return this;
        }
    }
}
