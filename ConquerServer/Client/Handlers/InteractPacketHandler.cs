using ConquerServer.Database.Models;
using ConquerServer.Combat;
using ConquerServer.Network;
using ConquerServer.Network.Packets;

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
            using (InteractPacket ip = new InteractPacket(p))
            {
                GameClient? target;

                switch (ip.Action)
                {
                    case InteractAction.Shoot:
                    case InteractAction.Attack:
                        {
                            if (World.TryGetPlayer(ip.TargetId, out target) && target != null)
                            {
                                Battle? battle = Battle.Create(this, target);
                                if (battle != null)
                                    await battle.Start();
                            }
                            break;
                        }
                    case InteractAction.CastMagic:
                        {
                            target = this;
                            if (ip.TargetId != ip.SenderId)
                                World.TryGetPlayer(ip.TargetId, out target);

                            //
                            // (  spell level[upper 16 bits] spellId[ lower 16 bits ] ) & ushort.MaxValue = [ lower 16 bits ]
                            // extract the lower 16 bits from data0 which contains the spellId
                            //
                            int spellId = ip.Data[0] & ushort.MaxValue;

                            MagicTypeModel spell = this.Magics[spellId].Attributes;
                            Battle? battle = Battle.Create(this, target, ip.X, ip.Y, spell);
                            if (battle != null)
                                await battle.Start();

                            break;
                        }

                }
            }
        }
    }
}
