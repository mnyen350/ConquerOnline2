using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;
using ConquerServer.Enemy;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [SlashCommand("test")]
        private void SlashTest(string[] messageContents) 
        {
            //var monster = new Monster();
            //monster.Lookface = new Lookface(200);
            //monster.MapId = this.MapId;
            //monster.X = this.X;
            //monster.Y = this.Y;
            //monster.Id = 500000;

            var spawnCircle = new MonsterSpawnManager();
            spawnCircle.Generate();

            // test?
            foreach (var m in spawnCircle.Monsters)
            {
                this.Send(Entity.CreateEntityPacket(m));
            }
        }

        [SlashCommand("resource")]
        private void SlashResource (string[] messageContents)
        {
            this.Stamina = 100;
            this.Mana = this.MaxMana;
            this.SendSynchronize();
        }
    }
}
