using ConquerServer.Client;
using ConquerServer.Database.Models;
using ConquerServer.Network;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public partial class Battle
    {
        private static MagicDispatcher c_Magic;
        static Battle()
        {
            c_Magic = new MagicDispatcher();
        }
        //source, target, skill/magic/lackof
        public GameClient Source { get; set; }
        public List<GameClient> Targets { get; set; }

        public MagicTypeModel? Spell { get; set; }
        public int CastX { get; private set; } //coordinate clicked to cast skill
        public int CastY { get; private set; }

        public Battle(GameClient source, GameClient target, int castX=0, int castY=0, MagicTypeModel? spell = null)
        {
            Source = source;
            Targets = new List<GameClient>();
            if (target != null)
                Targets.Add(target);
            Spell = spell;
            CastX = castX;
            CastY = castY;
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
            if (Spell == null)
                return;

            if (CastX == 0 && CastY == 0 && Targets.Count > 0)
            {
                CastX = Targets[0].X;
                CastY = Targets[0].Y;
            }

            var findTargets = c_Magic[Spell.Sort];
            if (findTargets == null)
            {
                Source.SendSystemMessage("Spell requested failed, magic type not handled");
                return;
            }
            
            // find elligible targets to be hit
            findTargets(this);
            // filter these targets
            FilterTargets();
            // process doing the damage now to them
            ProcessTargets();
        }

        private void FilterTargets()
        {
            // TO-DO: ...

            //dont attack yourself... at all
            if(Targets.Contains(Source))
            {
                Targets.Remove(Source);
            }
        }

        private void ProcessTargets()
        {
            if (Spell == null) return;

            // calculate damage done to each target, and send the damage packet to each target
            using (var p = new MagicEffectPacket()
                            .Begin(Source.Id, CastX, CastY, Spell.Type, Spell.Level, 0))
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
