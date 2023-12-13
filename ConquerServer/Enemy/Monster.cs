using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;
using ConquerServer.Database.Models;
using ConquerServer.Database;
using ConquerServer.Client;
using System.Collections;

namespace ConquerServer.Enemy
{

    public class Monster : Entity
    {
        public int MinGold { get; private set; }
        public int MaxGold { get; private set; }
        public MagicTypeModel? Magic { get; private set; }
        public Monster()
        {
            
        }
        public Monster(MonsterTypeModel monsterData)
        {
            Id= Db.GetNextMonsterId();
            Lookface = new Lookface(200);
            Level = 25;
            MaxHealth = 200;

            MinPhysicalAttack= 10;
            MaxPhysicalAttack= 10;
            MagicAttack = 10;
            
            PhysicalDefense= 10;
            MagicDefense = 10;

            MinGold = 10;
            MaxGold = 1000;
            Magic = null;
        }
    }
}
