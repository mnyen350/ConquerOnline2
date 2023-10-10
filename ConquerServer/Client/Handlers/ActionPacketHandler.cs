using ConquerServer.Database.Models;
using ConquerServer.Network;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;
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
        private async Task ActionPacketHandler(Packet p)
        {
            using (ActionPacket ap = new ActionPacket(p))
            {
                if (ap.Action == ActionType.Init_Map)
                {
                    using (ActionPacket act = new ActionPacket(MapId, X, Y, 0, ActionType.Init_Map, MapId))
                        Send(act);

                    using (ActionPacket act = new ActionPacket(MapId, X, Y, 0, ActionType.DisableDie, MapId))
                        Send(act);

                    // TO-DO: send guild info
                    // TO-DO: send hangup

                    FieldOfView.Move(this.MapId, this.X, this.Y); // update FOV
                }
                else if (ap.Action == ActionType.Init_Items)
                {
                    foreach (var item in Inventory)
                    {
                        this.SendItemInfo(item, ItemInfoAction.AddItem);
                    }

                    foreach (var equipment in Equipment)
                    {
                        this.SendItemInfo(equipment, ItemInfoAction.AddItem);

                        using (ItemUsePacket iup = new ItemUsePacket(ItemAction.Equip, equipment.Id, (int)equipment.Position))
                            Send(iup);
                    }

                    Equipment.Update(); // this will recalculate stats too
                    Send(p);
                }
                else if (ap.Action == ActionType.Init_Associates)
                {
                    Send(p);
                }
                else if (ap.Action == ActionType.Init_Proficiencies)
                {
                    foreach (var prof in Proficiencies.Values)
                        prof.Send();

                    Send(p);
                }
                else if (ap.Action == ActionType.Init_Spells)
                {
                    foreach (var magic in Magics.Values)
                        magic.Send();

                    Send(p);
                }
                else if (ap.Action == ActionType.Init_Guild) // last step of login seq
                {
                    this._loginSequenceCompleted = true;

                    Send(p);
                }
                else if (ap.Action == ActionType.Jump)
                {
                    // x,y is contained within data1
                    int nx = (int)(ap.Data & 0xffff);
                    int ny = (int)((ap.Data >> 16) & 0xffff);

                    SanityHelper.Validate(() => this.Id == ap.Id, "UID is not equal to the player");

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
                else if (ap.Action == ActionType.RequestEntity)
                {
                    //data1 == id , pull player havinf that id
                    GameClient oClient;
                    if (World.TryGetPlayer((int)ap.Data, out oClient) &&
                        oClient.MapId == this.MapId)
                    {
                        using (var spawn = CreateEntityPacket(oClient))
                            this.Send(spawn);
                    }
                    //purpose -> client: server recieved MISSING info
                    //"here is id of what info i need, spawn for me" - client
                }
                else if (ap.Action == ActionType.EnterPortal)
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
                else if (ap.Action == ActionType.ChangePkMode)
                {
                    Console.WriteLine(p.Dump("Unknown Action - " + ap.Action));

                    //data1 
                    PKMode pk = (PKMode)ap.Data;
                    if (pk.IsDefined())
                    {
                        this.PKMode = pk;
                        this.Send(p);
                    }
                }
                else if (ap.Action == ActionType.Revive)
                {
                    if (!CanRevive || !IsDead) return;

                    //remove ghostface from lookface
                    Lookface = Lookface.Normalize();

                    //remove "death" and "ghost" flags from status
                    Status -= StatusFlag.Ghost + StatusFlag.Death;

                    //teleport player to the revive coordinates
                    RevivePointModel rpm = Database.GetRevivePoint();
                    this.Teleport(rpm.ReviveMapId, rpm.X, rpm.Y);

                    //refill health
                    this.Health = this.MaxHealth;

                    //syncronize the stats with client
                    this.SendSynchronize(true);
                }
                else
                {
                    //Console.WriteLine(p.Dump("Unknown Action - " + mode));
                }
            }
        }
    }
}
