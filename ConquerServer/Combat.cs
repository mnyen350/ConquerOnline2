using ConquerServer.Client;
using ConquerServer.Database.Models;
using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public class Combat
    {
        //source, target, skill/magic/lackof
        public GameClient Source { get; set; }
        public List<GameClient> Targets { get; set; }
        public MagicTypeModel? Spell { get; set; }

        public Combat(GameClient source, GameClient target, MagicTypeModel? spell = null)
        {
            Source = source;
            Targets = new List<GameClient>();
            Targets.Add(target);
            Spell = spell;
        }

        public void Start()
        {
            if (Spell == null)
            {
                StartMelee();
            }
            else
            {
                StartMagic();
            }
        }

        private void StartMelee()
        {

        }

        private void StartMagic()
        {
            switch (Spell.Sort)
            {
                case MagicSort.Bomb:
                    {
                        StartBombMagic();
                        break;
                    }
            }
        }

        private void StartBombMagic()
        {
            // determine x,y of center of "bomb"
            int x = Source.X;
            int y = Source.Y;
            // find range
            int range = Spell.Range;
            // find all targets in range
            // TO-DO: circle
            Targets.AddRange(Source.FieldOfView.Where(p => p.Distance(Source) <= range));

            // filter targets.. only hitting "safe" targets
            // pk mode
            // alive/dead
            //exclude self
            // TO-DO: ...

            // calculate damage done to each target, and apply the damage to each target
            using (var p = new MagicEffectPacket()
                            .Begin(Source.Id, x, y, Spell.Type, Spell.Level, 0))
            {
                foreach (var entity in Targets)
                {
                    int damage = 123;
                    bool miss = false;
                    p.Add(entity.Id, damage, miss ? 0 : 1, 0);
                }

      
                Source.FieldOfView.Send(p.End(), true);
            }



        }


    }
}
