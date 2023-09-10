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
        [Network(PacketType.Walk)]
        private void WalkPacketHandler(Packet p)
        {
            int direction = p.ReadInt32();
            int uid = p.ReadInt32();
            int mode = p.ReadInt32(); // walk/run/mount(9)
            uint timestamp = p.ReadUInt32();
            int mapId = p.ReadInt32();

            direction = direction % 8;

            //log distance, make enum of directions, number that client send you
            //offset the coodrinate by the appropriate 0-1 change? 

            FieldOfView.Send(p, true);

            int nx, ny;
            MathHelper.GetWalkCoordinate(X, Y, (Direction)direction, out nx, out ny);
            FieldOfView.Move(this.MapId, nx, ny);
        }
    }
}
