using ConquerServer.Database.Models;
using ConquerServer.Combat;
using ConquerServer.Network;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [Network(PacketType.Interact)]
        private async Task InteractPacketHandler(Packet p)
        {
            uint timestamp1 = p.ReadUInt32();               // 4 (5735)
            uint timestamp2 = p.ReadUInt32();               // 8
            int senderId = p.ReadInt32();                   // 12
            int targetId = p.ReadInt32();                   // 16
            int x = p.ReadInt16();                          // 20
            int y = p.ReadInt16();                          // 22
            var action = (InteractAction)p.ReadInt32();     // 24
            int data0 = p.ReadInt32();                      // 28
            int data1 = p.ReadInt32();                      // 32
            int data2 = p.ReadInt32();                      // 36
            int data3 = p.ReadInt32();                      // 40


            GameClient? target;

            switch (action)
            {
                case InteractAction.Shoot:
                case InteractAction.Attack:
                    {
                        if (World.TryGetPlayer(targetId, out target))
                        {
                            Battle battle = new Battle(this, target);
                            await battle.Start();
                        }
                        break;
                    }
                case InteractAction.CastMagic:
                    {
                        // magic attacks
                        DecodeMagicAttack(senderId, ref data0, ref targetId, ref x, ref y);
                        data0 &= ushort.MaxValue;

                        Console.WriteLine("{0} {1} {2} {3} {4}", senderId, data0, targetId, x, y);

                        target = this;
                        if (targetId != senderId)
                            World.TryGetPlayer(targetId, out target);
                        

                        //typeid -> magictypemodel
                        //source.magic[typeid].attri
                        MagicTypeModel spell = this.Magics[data0].Attributes;
                        Battle battle = new Battle(this, target, x, y, spell);
                        await battle.Start();
                        break;
                    }

            }
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

        private static void DecodeMagicAttack(int senderId, ref int data0, ref int targetId, ref int posX, ref int posY)
        {
            int magicType, magicLevel;
            MathHelper.BitUnfold32(data0, out magicType, out magicLevel);

            magicType = (ushort)(ExchangeShortBits(((ushort)magicType ^ (uint)senderId ^ 0x915d), 16 - 3) + 0x14be);
            magicLevel = (ushort)(((byte)magicLevel) ^ 0x21);

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
