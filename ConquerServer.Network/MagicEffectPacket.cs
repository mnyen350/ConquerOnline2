using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network
{
    public class MagicEffectPacket : Packet
    {
        private int startOffset;
        private int id, data, type, lv, effect;

        public MagicEffectPacket()
            : base(4096)
        {
            startOffset = 0;
        }

        private void InternalBuild()
        {
            WriteInt16(startOffset + 2, (short)PacketType.MagicEffect);
            WriteInt16(startOffset, (short)(this.Offset - startOffset));
            Size = Offset;
        }

        private unsafe short GetCount()
        {
            return *((short*)&this.Stream[startOffset + 17]);
        }

        private unsafe void IncrementCount()
        {
            *((short*)&this.Stream[startOffset + 17]) += 1;
        }

        public MagicEffectPacket Begin(int id, int data, int type, int lv, int effect)
        {
            this.id = id;
            this.data = data;
            this.type = type;
            this.lv = lv;
            this.effect = effect;

            if (startOffset > 0) // not the first packet
            {
                this.WriteCString("TQServer", 8); // previous packet padding
                this.WriteInt32(0); // space for the next size,type
            }

            this.WriteInt32(id);
            this.WriteInt32(data);
            this.WriteInt16((short)type);
            this.WriteInt16((short)lv);
            //this.WriteInt32(0);
            this.WriteInt8((byte)effect);
            this.WriteInt8(0);
            this.WriteInt8(0);
            this.WriteInt8(0);

            return this;
        }

        public MagicEffectPacket Begin(int id, int type, int lv, int effect)
        {
            return Begin(id, 0, type, lv, effect);
        }

        public MagicEffectPacket Begin(int id, int x, int y, int type, int lv, int effect)
        {
            return Begin(id, (y << 16) | x, type, lv, effect);
        }

        public MagicEffectPacket Add(int id, params int[] data)
        {
            IncrementCount();
            WriteInt32(id);
            int i;
            for (i = 0; i < data.Length && i < 7; i++)
                WriteInt32(data[i]);
            for (; i < 7; i++)
                WriteInt32(0);

            if (GetCount() >= 10)
            {
                //Console.WriteLine("Splitting attack packet...");
                End();

                Begin(this.id, this.data, type, lv, effect); // remark
            }

            return this;
        }

        public MagicEffectPacket End()
        {
            InternalBuild();
            startOffset = Offset + 8; // next start offset
            return this;
        }
    }
}
