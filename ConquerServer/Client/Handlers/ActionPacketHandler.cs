using ConquerServer.Database.Models;
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
        [Network(PacketType.Action)]
        private void ActionPacketHandler(Packet p)
        {
            uint timestamp1 = p.ReadUInt32();       // 4    int (supposedly always set)
            int uid = p.ReadInt32();                // 8    int
            long data1 = p.ReadInt64();             // 12   long (short/short/int)
            uint timestamp2 = p.ReadUInt32();       // 20   int (can be zero)
            var mode = (ActionType)p.ReadInt16();   // 24   short
            int dir = p.ReadInt16();                // 26   short
            int data2 = p.ReadInt32();              // 28   int (short/short)
            int data3 = p.ReadInt32();              // 32   int
            int data4 = p.ReadInt32();              // 36   int
            bool charging = p.ReadInt8() != 0;      // 40   byte (=1 when charging?)
            string[] strings = p.ReadStrings();     // 41   string_list

            if (mode == ActionType.Init_Map)
            {
                using (var p1 = CreateAction(MapId, X, Y, 0, ActionType.Init_Map, MapId))
                    Send(p1);

                using (var p1 = CreateAction(MapId, X, Y, 0, ActionType.DisableDie, MapId))
                    Send(p1);

                // TO-DO: send guild info
                // TO-DO: send hangup

            }
            else if (mode == ActionType.Init_Items)
            {
                Send(p);
            }
            else if (mode == ActionType.Init_Associates)
            {
                Send(p);
            }
            else if (mode == ActionType.Init_Proficiencies)
            {
                Send(p);
            }
            else if (mode == ActionType.Init_Spells)
            {
                Send(p);
            }
            else if (mode == ActionType.Init_Guild) // last step of login seq
            {
                Send(p);
            }
            else if (mode == ActionType.Jump)
            {
                // x,y is contained within data1
                int nx = (int)(data1 & 0xffff);
                int ny = (int)((data1 >> 16) & 0xffff);

                SanityHelper.Validate(() => this.Id == uid, "UID is not equal to the player");

                // find the distance between (cx, cy) to (nx, ny) via two point formula -> MATHEHELPER.GETDISTANCE()
                // if distance is greater than 18, it's an invalid jump (for now disconnect the player) ->  SANITYHELPER.VALIDATEJUMP()
                SanityHelper.ValidateJump(X, Y, nx, ny);


                //send jump packet to players in old range
                FieldOfView.Send(p, true);     
                /*
                 * problem is that p1 is movcing out of p2's view
                 * because fov updates and then sends JUMP, p2 never receive's JUMP
                 * 
                 * JUMP must get sent to both p1's fov (prior to moving)
                 * and JUMP must get sent to any new player's added to p1's fov (after p1 has been spawned to p3)
                 */

                //update _screen
                FieldOfView.Move(this.MapId, nx, ny);
            }
            else if (mode == ActionType.RequestEntity)
            {
                //data1 == id , pull player havinf that id
                GameClient oClient;
                if(World.TryGetPlayer((int)data1, out oClient) &&
                    oClient.MapId == this.MapId)
                {
                    using (var spawn = CreateEntityPacket(oClient))
                        this.Send(spawn);
                }
                //purpose -> client: server recieved MISSING info
                                    //"here is id of what info i need, spawn for me" - client
            }
            else if(mode== ActionType.EnterPortal)
            {
                /*
                 * check player's location for +-5 of portal locations
                 * 
                 * check player's mapid == portal.FROMmapid
                 *       player x == fromx +-5
                 *       
                 */
                PortalModel? portal = Database.Portals.FirstOrDefault(p => p.Distance(this) <= 5);
                if (portal == null)
                {
                    Console.WriteLine("no portal found");
                    return; 
                }

                Console.WriteLine("portal found: ");
                Console.WriteLine($"{portal.X}, {portal.Y}, {MapId} going to {portal.ToX}, {portal.ToY}, {portal.ToMapId}");

                this.Teleport(portal.ToMapId, portal.ToX, portal.ToY);
            }
            else
            {
                //Console.WriteLine(p.Dump("Unknown Action - " + mode));
            }
        }
    }
}
