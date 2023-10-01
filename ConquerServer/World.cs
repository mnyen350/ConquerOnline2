using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Client;

namespace ConquerServer
{
    public class World
    {
        //playerid, game client
        private static ConcurrentDictionary<int, GameClient> _gameClients;

        static World()
        {
            _gameClients = new ConcurrentDictionary<int, GameClient>();
        }

        public GameClient[] Players {  get { return _gameClients.Values.ToArray(); } }

        public GameClient Owner { get; private set; }
        public World(GameClient owner)
        {
            Owner = owner;
        }  

        public void AddPlayer(GameClient? client = null)
        {
            client = client?? Owner;
            _gameClients.TryAdd(client.Id, client);
        }

        public void RemovePlayer(int? id = null)
        {
            id = id ?? Owner.Id;    
            GameClient? player;
            if (_gameClients.TryRemove((int)id, out player))
            {
                player.FieldOfView.Despawn();
            }
        }

        public bool TryGetPlayer(int id, out GameClient? client)
        {
            return _gameClients.TryGetValue(id, out client);
        }

        public bool TryGetPlayer(string name, out GameClient? client)
        {
            client= Players.FirstOrDefault(g => g.Name == name);
            return (client != null);
        }
    }
}
