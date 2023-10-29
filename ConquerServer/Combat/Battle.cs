using ConquerServer.Client;
using ConquerServer.Database.Models;
using ConquerServer.Network;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
        public HashSet<GameClient> Targets { get; set; }

        public MagicTypeModel? Spell { get; set; }
        public int CastX { get; private set; } //coordinate clicked to cast skill
        public int CastY { get; private set; }

        public Battle(GameClient source, GameClient target, int castX=0, int castY=0, MagicTypeModel? spell = null)
        {
            Source = source;
            Targets = new HashSet<GameClient>();
            if (target != null)
                Targets.Add(target);
            Spell = spell;
            CastX = castX;
            CastY = castY;
        }

        public async Task Start()
        {
            if (Spell == null)
            {
                await StartMelee();
            }
            else
            {
                await StartMagic();
            }
        }

        private async Task StartMelee()
        {
            // determine first, should a passive skill activate instead?
            if (!await ActivatePassiveSkill())
            {
                FilterTargets();
                ProcessTargets();
            }
        }

        private async Task StartMagic()
        {
            if (Spell == null)
                return;

            if (CastX == 0 && CastY == 0 && Targets.Count > 0)
            {
                var firstTarget = Targets.First();
                CastX = firstTarget.X;
                CastY = firstTarget.Y;
            }

            var findTargets = c_Magic[Spell.Sort];
            if (findTargets == null)
            {
                Source.SendSystemMessage($"Spell {Spell.Type} requested failed, magic sort {Spell.Sort} not handled");
                return;
            }

            if (Source.NextMagic > DateTime.Now)
                return; //do nothing when its not time to cast magic again

            await Task.Delay(Spell.DelayCast);
            Source.NextMagic = DateTime.Now.AddMilliseconds(Spell.DelayNextMagic);


            // find elligible targets to be hit
            findTargets(this);
            // filter these targets
            FilterTargets();    
            // process doing the damage now to them

            ProcessTargets();
        }

        private IEnumerable<MagicTypeModel> GetPassiveMagic()
        {
            // get what's in the hand
            var w1 = Source.Equipment[ItemPosition.Set1Weapon1]?.SubType;
            var w2 = Source.Equipment[ItemPosition.Set1Weapon2]?.SubType;

            // then filter by WeaponSubType

            return Source.Magics.Values
                .Select(m => m.Attributes)
                .Where(m => m.IsWeaponPassive)
                .Where(m => m.WeaponSubType == w1 || m.WeaponSubType == w2);
        }

        private async Task<bool> ActivatePassiveSkill()
        {
            // get available passive magic
            foreach (var passive in GetPassiveMagic())
            {
                //rng a number 
                int rng = Utility.Random.Next(0, 100);

                //if <= then skill suceeded
                if (rng <= passive.Accuracy)
                {
                    Spell = passive;
                    await StartMagic();
                    return true;
                }           
            }
            return false;
        }

        private bool IsPotentialTarget(GameClient target)
        {
            if (target.Id == Source.Id) return false; // cannot hit yourself

            if (target.Health <= 0) return false; // cannot hit dead

            if (Source.PKMode == PKMode.Peace) 
            {
                return false; //hitting no one
            }
            else if(Source.PKMode == PKMode.Capture)
            {
                //TO-DO: implement nameflashing(datetime??), black name(PK POINT SYSTEM)
                return false;
            }
            else if(Source.PKMode == PKMode.Revenge)
            {
                //TO-DO: implement revenge list, auto add ppl who just killed you.. only hit those ppl
                return false;
            }
            else if(Source.PKMode == PKMode.Team)
            {
                //TO-DO: implrement Team like literally the team of 4 u are in RN
                return false;
            }
            else if(Source.PKMode == PKMode.Guild)
            {
                //TO-DO: everyone EXCEPT, guild, team, guild's allies
                return false;
            }
            /*else if(Source.PKMode == PKMode.Kill)
            {
                return true;
            }*/

            return true; // is potential target
        }

        private void FilterTargets()
        {
            Targets.RemoveWhere(t => !IsPotentialTarget(t));
        }

        private void BroadcastDeath(GameClient target)
        {
            int data0 = MathHelper.BitFold32(Spell != null ? 3 : 1, 0);
            using (InteractPacket ip = new InteractPacket(Source.Id, target.Id, target.X ,target.Y, InteractAction.Kill, data0, 0,0,0))
                target.FieldOfView.Send(ip, true);
           
        }

        private bool IsDodgeDamage()
        {
            if (Source.Equipment[ItemPosition.Set1Weapon1]?.SubType == ItemType.Bow && 
                Source.Equipment[ItemPosition.Set1Weapon2]?.SubType == ItemType.Arrow)
            {
                return true;
            }
            return false;
        }

        private DamageAlgorithm GetDamageAlgorithm(GameClient target)
        {
            if (Spell != null && Spell.UseMP > 0)
            {
                return new MagicAlgorithm(Source, target, Spell);
            }
            else if (IsDodgeDamage())
            {
                return new DodgeAlgorithm(Source, target, Spell);
            }
            else
            {
                return new PhysicalAlgorithm(Source, target, Spell);
            }
        }

        private void ProcessTargets()
        {
         
            var damage = new Dictionary<int, (int, bool)>();

            foreach (var target in Targets)
            {
                var algorithm = GetDamageAlgorithm(target);
                var hit = algorithm.Calculate();
                target.Health -= hit;
                damage.Add(target.Id, (hit, false));
            }

            if (Spell != null)
            {
                // send the damage packet to each target
                using (var p = new MagicEffectPacket()
                                .Begin(Source.Id, CastX, CastY, Spell.Type, Spell.Level, 0))
                {
                    foreach (var kvp in damage)
                        p.Add(kvp.Key, kvp.Value.Item1, kvp.Value.Item2 ? 0 : 1, 0);
                    
                    Source.FieldOfView.Send(p.End(), true);
                }
            }
            else
            {
                // TO-DO: archer damage
                foreach (var kvp in damage)
                {
                    var action = InteractAction.Attack;
                    using (InteractPacket ip = new InteractPacket(Source.Id, kvp.Key, Source.X, Source.Y, action, kvp.Value.Item1, 0, 0, 0))
                        Source.FieldOfView.Send(ip, true);
                }
            }

            // sync all entites involved including Source
            foreach (var entity in Targets.Concat(new[] { Source }))
            {
                if (entity.IsDead)
                {
                    // add the death status flag
                    entity.Status += StatusFlag.Death;

                    // issue the death broadcast, and change to ghost
                    Utility.Delay(TimeSpan.FromSeconds(0.15), async () =>
                    {
                        if (!entity.IsDead) return; // revaldiate our assumption
                        BroadcastDeath(entity);

                        await Task.Delay(TimeSpan.FromSeconds(1.5));

                        if (!entity.IsDead) return; // revaldiate our assumption
                        entity.Status += StatusFlag.Ghost;
                        entity.Lookface = entity.Lookface.ToGhost();
                        entity.SendSynchronize(true);
                        Console.WriteLine($"{entity.Name} {entity.Lookface.ToUInt32()}");

                        await Task.Delay(TimeSpan.FromSeconds(16.5));

                        if (!entity.IsDead) return;
                        entity.CanRevive = true;
                    });
                }

                entity.SendSynchronize(true);
            }
        }
    }
}
