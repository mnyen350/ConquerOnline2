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
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Combat
{
    public enum PotentialTargetError
    {
        OK,
        SelfHarm,
        Grounded,
        Peace,
        Capture,
        Revenge,
        Team,
        Guild,
        XPDefense,
        TargetGhost
    }
    public class Battle
    {
        static Battle()
        {
            //c_Magic = new MagicDispatcher();
        }

        //source, target, skill/magic/lackof
        public Entity Source { get; private set; }
        public GameClient? SourceClient { get; private set; }

        public HashSet<Entity> Targets { get; set; }

        protected virtual bool IsOffensive => true;
        protected virtual bool IsGrounded => true;
        protected virtual bool AllowDeadTarget => false;

        public Battle(Entity source, Entity? target)
        {
            Source = source;
            if (source.IsPlayer)
                SourceClient = (GameClient)source;

            Targets = new HashSet<Entity>();
            if (target != null)
                Targets.Add(target);
        }

        public static Battle? Create(Entity source, Entity? target, int castX = 0, int castY = 0, MagicTypeModel? spell = null)
        {
            if (spell != null)
            {
                switch (spell.Sort)
                {
                    case MagicSort.AddMana: return new AddManaBattle(source, target, castX, castY, spell);

                    case MagicSort.AttachStatus: return new AttachStatusBattle(source, target, castX, castY, spell);
                    
                    case MagicSort.AttackStatus: 
                    case MagicSort.Attack: return new MagicBattle(source, target, castX, castY, spell);
                    
                    case MagicSort.BombGroundTarget:
                    case MagicSort.Bomb: return new BombBattle(source, target, castX, castY, spell);
                    
                    case MagicSort.Fan: return new FanBattle(source, target, castX, castY, spell);
                    
                    case MagicSort.TargetedLine:
                    case MagicSort.Line: return new LineBattle(source, target, castX, castY, spell);
                    
                    case MagicSort.Recruit: return new RecruitBattle(source, target, castX, castY, spell);
                    
                    case MagicSort.DetachStatus: return new DetachStatusBattle(source, target, castX, castY, spell);

                    case MagicSort.DecLife: return new DecLifeBattle(source, target, castX, castY, spell);

                    default:
                        {
                            if (source.IsPlayer)
                                ((GameClient)source).SendSystemMessage($"Spell {spell.Type} requested failed, magic sort {spell.Sort} not handled");
                            break;
                        }
                }

                // unknown battle, break things
                return null;
            }
            else
            { 
                return new Battle(source, target);
            }
        }

        public virtual async Task Start()
        {
            FilterTargets();
            // determine first, should a passive skill activate instead?
            if (!await ActivatePassiveSkill())
            {
                ProcessTargets();
                SynchronizeAll();
            }
        }


        protected virtual void FindTargets()
        {
            return;
        }


        private IEnumerable<MagicTypeModel> GetPassiveMagic()
        {
            if (SourceClient != null)
            {
                // get what's in the hand
                var w1 = SourceClient.Equipment[ItemPosition.Set1Weapon1]?.SubType;
                var w2 = SourceClient.Equipment[ItemPosition.Set1Weapon2]?.SubType;

                // then filter by WeaponSubType

                return SourceClient.Magics.Values
                    .Select(m => m.Attributes)
                    .Where(m => m.IsWeaponPassive)
                    .Where(m => m.WeaponSubType == w1 || m.WeaponSubType == w2);
            }
            return new MagicTypeModel[0];
        }

        private async Task<bool> ActivatePassiveSkill()
        {
            // get available passive magic
            foreach (var passive in GetPassiveMagic())
            {
                    foreach (var target in Targets) 
                    {
                    //rng a number 
                    int rng = Utility.Random.Next(0, 100);
                    //if <= then skill suceeded
                    if (rng <= passive?.Accuracy)
                    {
                        Battle? passiveBattle = Battle.Create(this.Source, target, target.X, target.Y, passive);
                        if (passiveBattle != null)
                        {
                            await passiveBattle.Start();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        protected virtual PotentialTargetError IsCorrectPKTarget()
        {
            if (Source.PKMode == PKMode.Peace)
            {
                return PotentialTargetError.Peace; //hitting no one
            }
            else if (Source.PKMode == PKMode.Capture)
            {
                //TO-DO: implement nameflashing(datetime??), black name(PK POINT SYSTEM)
                return PotentialTargetError.Capture;
            }
            else if (Source.PKMode == PKMode.Revenge)
            {
                //TO-DO: implement revenge list, auto add ppl who just killed you.. only hit those ppl
                return PotentialTargetError.Revenge;
            }
            else if (Source.PKMode == PKMode.Team)
            {
                //TO-DO: implrement Team like literally the team of 4 u are in RN
                return PotentialTargetError.Team;
            }
            else if (Source.PKMode == PKMode.Guild)
            {
                //TO-DO: everyone EXCEPT, guild, team, guild's allies
                return PotentialTargetError.Guild;
            }
            /*else if(Source.PKMode == PKMode.Kill)
            {
                return true;
            }*/
            return PotentialTargetError.OK;
        }
        protected virtual PotentialTargetError IsPotentialTarget(Entity target)
        {
            //no punch punch ghosts
            if (target.IsDead && !AllowDeadTarget)
                return PotentialTargetError.TargetGhost;

            if (IsOffensive)
            {

                // cannot hit yourself
                if (target.Id == Source.Id) return PotentialTargetError.SelfHarm;

                //must be in PK mode (kill)
                var res = IsCorrectPKTarget();
                if (res != PotentialTargetError.OK)
                    return res;
            }

            // if flying no melee other than archer 
            if (IsGrounded && target.Status.IsAttached(StatusType.Fly))
            {
                if (!IsDodgeDamage())
                    return PotentialTargetError.Grounded;
            }

            return PotentialTargetError.OK;
        }

        protected void FilterTargets()
        {
            Targets.RemoveWhere(t =>
            {
                var result = IsPotentialTarget(t);
                if (result !=  PotentialTargetError.OK)
                {
                    Console.WriteLine("target invalid due to {0}", result);
                    return true;
                }
                return false;
            });
        }

        protected virtual bool IsDodgeDamage()
        {
            if (SourceClient != null)
            {
                var w1 = SourceClient.Equipment[ItemPosition.Set1Weapon1]?.SubType;
                var w2 = SourceClient.Equipment[ItemPosition.Set1Weapon2]?.SubType;
                if (w1 == ItemType.Bow && w2 == ItemType.Arrow)
                    return true;
            }
            return false;
        }

        protected virtual DamageAlgorithm GetDamageAlgorithm(Entity target)
        {
            //if (Spell != null && Spell.UseMana > 0)
            //{
            //    return new MagicAlgorithm(Source, target, Spell);
            //}
            if (IsDodgeDamage())
            {
                return new DodgeAlgorithm(Source, target, null);
            }
            else
            {
                return new PhysicalAlgorithm(Source, target, null);
            }
        }

        protected virtual void ProcessTarget(Entity target, Dictionary<int, (int, bool)> power)
        {
            var algorithm = GetDamageAlgorithm(target);
            var hit = algorithm.Calculate();
            target.Health -= hit;
            power.Add(target.Id, (hit, false));
        }

        protected void ProcessTargets()
        {
            var power = new Dictionary<int, (int, bool)>();
            foreach (var target in Targets)
                ProcessTarget(target, power);

            SendDisplay(power);
        }

        protected virtual void SendDisplay(Dictionary<int, (int, bool)> power)
        {
            foreach (var kvp in power)
            {
                var action = IsDodgeDamage() ? InteractAction.Attack : InteractAction.Shoot;
                using (InteractPacket ip = new InteractPacket(Source.Id, kvp.Key, Source.X, Source.Y, action, kvp.Value.Item1, 0, 0, 0))
                    Source.FieldOfView.Send(ip, true);
            }
        }

        protected void SynchronizeAll()
        {
            // sync all entites involved including Source
            foreach (var entity in Targets.Concat(new[] { Source }))
            {
                if (entity.IsDead)
                {
                    // add the death status
                    entity.Status.AttachDeath(Source);
                }

                entity.SendSynchronize();
            }
        }
    }
}
