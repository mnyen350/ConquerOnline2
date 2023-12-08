using ConquerServer.Network;
using ConquerServer.Shared;
using System.Collections;
using System.Collections.Concurrent;

namespace ConquerServer.Client
{
    public class FieldOfView : IEnumerable<Entity>
    {
        //POV is 19 DISTANCE in all directions
        private const int MAX_FOV_DISTANCE = 32;
        private ConcurrentDictionary<int, Entity> _screen;
        public Entity Owner { get; private set; }
        


        public FieldOfView(Entity client) 
        {   
            Owner = client;
            _screen = new ConcurrentDictionary<int, Entity>();
           
        }
        public IEnumerator<Entity> GetEnumerator() => _screen.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _screen.Values.GetEnumerator();
        public void Clear()
        {
            //
            // handle exiting someone's fov
            //
            this.Despawn();
            _screen.Clear();
        }

        public void Despawn()
        {
            foreach (var p2 in _screen.Values)
            {
                p2.FieldOfView._screen.Remove(Owner.Id);
                using (var remove = Entity.CreateDespawnPacket(Owner))
                    p2.Send(remove);
            }
        }

        /*
         * left x
         * right x
         * 
         * 
         * 
         * top y
         * bottom y 
         * 
         * gameclients[] check x.y within range
         * if yes, createntity packet
         * send to owner to spawn everything/one in range
         */

        /*
         * UPDATE() to be called by walk/jump/login
         */
        public void Move(int mapId, int nx, int ny)
        {
            Owner.Emote = EmoteType.None;
            /* owner moves(?)
             * 
             * find new things the owner can see(?)
             * LOOP -> world.Players
             *      search if player mapid == 
             *      AND 
             *      if other player coordinate within 18 distance
             *      
             *      IF owner doesnt already see other player, "spawn the player" -> entitypacket? 
             *     
             */

            //
            // handle exiting someone's fov
            //
            foreach (var p2 in _screen.Values)
            {
                if (
                    // remove anything inside _screen which is no longer on the same map
                    Owner.MapId != mapId ||
                    // remove anything inside _screen which are no longer in range
                    MathHelper.GetDistance(p2.X, p2.Y, nx, ny) > MAX_FOV_DISTANCE)
                {
                    _screen.Remove(p2.Id);
                    p2.FieldOfView._screen.Remove(Owner.Id);

                    Console.WriteLine($"{Owner.Name} can no longer see {p2.Name}");
                    Console.WriteLine($"{p2.Name} can no longer see {Owner.Name}");

                    // forceful remove
                    if (mapId != p2.MapId)
                    {
                        using (var remove = GameClient.CreateDespawnPacket(Owner))
                            p2.Send(remove);
                    }
                }
            }


            //
            // update the owner's location
            //
            int oX = Owner.X;
            int oY = Owner.Y;
            int oMapId = Owner.MapId;
            Owner.MapId = mapId;
            Owner.X = nx;
            Owner.Y = ny;

            //
            // find new things ot add to _screen, which are now in range
            //
            foreach(var p2 in Owner.World.Players)
            {
                if (Owner.Id == p2.Id) continue; // cannot see self
                if (Owner.MapId != p2.MapId) continue; // must be the same mapid
                if (_screen.ContainsKey(p2.Id) && p2.FieldOfView._screen.ContainsKey(Owner.Id)) continue; // can already see each other
                if (MathHelper.GetDistance(p2.X, p2.Y, nx, ny) > MAX_FOV_DISTANCE) continue; // outside fov

                using (Packet p2Spawn = GameClient.CreateEntityPacket(p2),
                              p1Spawn = GameClient.CreateEntityPacket(Owner))
                {
                    p2.Send(p1Spawn);
                    Owner.Send(p2Spawn);
                }
    
                _screen.TryAdd(p2.Id, p2);
                p2.FieldOfView._screen.TryAdd(Owner.Id, Owner);

                Console.WriteLine($"{Owner.Name} can now see {p2.Name}");
                Console.WriteLine($"{p2.Name} can now see {Owner.Name}");
            }
        }

        public void Send(Packet p, bool sendSelf = false)
        {
            if (sendSelf)
                Owner.Send(p);

            foreach (var gc in _screen.Values)
                gc.Send(p);
        }
        
    }
}
