using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ConquerServer.Network.Packets
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
    public class InteractPacket : Packet
    {
        public uint Timestamp { get; set; }
        public uint Timestamp2 { get; set; }
        public int SenderId { get; set; }
        public int TargetId { get; set; }
        public int X { get ; set; }
        public int Y { get; set; }
        public InteractAction Action { get; set; }
        public int[] Data { get; set; }

        public InteractPacket(int senderId, int targetId,
            int x, int y, InteractAction action, params int[] data)
            :base(64)
        {
            Timestamp = TimeStamp.GetTime();
            Timestamp2 = TimeStamp.GetTime();
            SenderId = senderId;
            TargetId = targetId;
            X = x;
            Y = y;
            Action = action;
            Data = new int[4];

            int length = data.Length;
            for(int i = 0; i < length; i++)
                Data[i] = data[i];

            Build();
        }

        public InteractPacket(Packet p)
            :base(64)
        {
            int data0, targetId, x, y;

            Timestamp = p.ReadUInt32();
            Timestamp2 = p.ReadUInt32();
            SenderId = p.ReadInt32();
            targetId = p.ReadInt32();
            x =p.ReadInt16();
            y =p.ReadInt16();
            Action = (InteractAction)p.ReadInt32();
            Data = new int[4];
            data0 = p.ReadInt32();
            Data[1] = p.ReadInt32();
            Data[2] = p.ReadInt32();
            Data[3] = p.ReadInt32();

            if (Action == InteractAction.CastMagic)
                DecodeMagicAttack(SenderId, Timestamp, ref data0, ref targetId, ref x, ref y);

            Data[0] = data0;
            TargetId = targetId;
            X = x;
            Y = y;
        }

        public override void Build()
        {
            int data0 = Data[0];
            int targetId = TargetId;
            int x = X;
            int y = Y;

            if (Action == InteractAction.CastMagic)
                EncodeMagicAttack(SenderId, Timestamp, ref data0, ref targetId, ref x, ref y);

            WriteUInt32(Timestamp); // 5735
            WriteUInt32(Timestamp2);
            WriteInt32(SenderId);
            WriteInt32(targetId);
            WriteInt16((short)x);
            WriteInt16((short)y);
            WriteInt32((int)Action);
            WriteInt32(data0);
            WriteInt32(Data[1]);
            WriteInt32(Data[2]);
            WriteInt32(Data[3]);

            Build(PacketType.Interact);
        }

        private static uint ExchangeShortBits(uint data, int bits)
        {
            data &= 0xffff;
            return ((data >> bits) | (data << (16 - bits))) & 0xffff;
        }

        private static uint ExchangeLongBits(uint data, int bits)
        {
            return (data >> bits) | (data << (32 - bits));
        }

        private static void DecodeMagicAttack(int senderId, uint ts, ref int data0, ref int targetId, ref int posX, ref int posY)
        {
            int magicType, magicLevel;
            MathHelper.BitUnfold32(data0, out magicType, out magicLevel);

            magicType = (ushort)(ExchangeShortBits(((ushort)magicType ^ (uint)senderId ^ 0x915d), 16 - 3) + 0x14be);
            magicLevel = (ushort)((magicLevel ^ 0x3721) - (0x100 * (ts % 0x100)));


            data0 = MathHelper.BitFold32(magicType, magicLevel);
            targetId = (int)((ExchangeLongBits((uint)targetId, 13) ^ (uint)senderId ^ 0x5f2d2463) + 0x8b90b51a);
            posX = (short)(ExchangeShortBits(((ushort)posX ^ (uint)senderId ^ 0x2ed6), 16 - 1) + 0xdd12);
            posY = (short)(ExchangeShortBits(((ushort)posY ^ (uint)senderId ^ 0xb99b), 16 - 5) + 0x76de);
        }

        private static void EncodeMagicAttack(int senderId, uint ts, ref int data0, ref int targetId, ref int posX, ref int posY)
        {
            int magicType, magicLevel;
            MathHelper.BitUnfold32(data0, out magicType, out magicLevel);

            magicType = (ushort)(ExchangeShortBits((uint)magicType - 0x14be, 3) ^ senderId ^ 0x915d);
            magicLevel = (ushort)((magicLevel + 0x100 * (ts % 0x100)) ^ 0x3721);

            data0 = MathHelper.BitFold32(magicType, magicLevel);
            targetId = (int)ExchangeLongBits((((uint)targetId - 0x8b90b51a) ^ (uint)senderId ^ 0x5f2d2463u), 32 - 13);
            posX = (short)(ExchangeShortBits((uint)posX - 0xdd12, 1) ^ senderId ^ 0x2ed6);
            posY = (short)(ExchangeShortBits((uint)posY - 0x76de, 5) ^ senderId ^ 0xb99b);
        }
    }
}
