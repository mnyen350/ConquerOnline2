using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;
using ConquerServer.Database;
using ConquerServer.Database.Models;

namespace ConquerServer.Enemy
{
    public class SpawnCircleModel
    {
        public int X { get;  set; }
        public int Y { get;  set; }
        public int Radius { get;  set; }
        public int MapId { get;  set; }
        public int MaxCount { get; set; }   
        public SpawnCircleModel() 
        {

        }
    }


    public class MonsterSpawnManager
    {
        public SpawnCircleModel SpawnCircle { get; private set; } 
        public MonsterTypeModel MonsterData { get; private set;}

        private List<Monster> _monsters;
        public IEnumerable<Monster> Monsters => _monsters;

        public MonsterSpawnManager()
        {
            SpawnCircle = new SpawnCircleModel()
            {
                X = 50,
                Y = 50,
                Radius = 5,
                MapId = 1005,
                MaxCount = 2
            };

            //MonsterData = new MonsterTypeModel()
            //{
            //    Lookface = new Lookface(200),
            //    Level = 25,
            //    MaxHealth = 200,
            //    Attack = 10,
            //    MagicAttack = 10,
            //    Defense = 10,
            //    MagicDefense = 10,
            //    MinGold = 10,
            //    MaxGold = 1000,
            //    Magic = null
            //};

            _monsters = new List<Monster>(SpawnCircle.MaxCount);
        }

        public void Generate()
        {
            while(_monsters.Count < SpawnCircle.MaxCount)
            {
                Monster ms = new Monster(MonsterData);
                ms.X = SpawnCircle.X;
                ms.Y = SpawnCircle.Y;
                ms.MapId = SpawnCircle.MapId;
                _monsters.Add(ms);
            }

        }

        //private mee6 to spawn on loop assuming not array not filled
    }
}
