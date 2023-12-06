using ConquerServer.Client;
using ConquerServer.Database.Models;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;

namespace ConquerServer.Combat
{
    public class MagicBattle : Battle
    {
        public MagicTypeModel Spell { get; set; }
        public int CastX { get; private set; } //coordinate clicked to cast skill
        public int CastY { get; private set; }
        protected override bool IsOffensive => Spell.IsOffensive;
        protected override bool IsGrounded => Spell.IsGrounded;
        

        public MagicBattle(GameClient source, GameClient? target, int castX, int castY, MagicTypeModel spell)
            : base(source, target)
        {
            Spell = spell;
            CastX = castX;
            CastY = castY;
        }

        protected override bool IsDodgeDamage()
        {

            if (Spell.WeaponSubType == ItemType.Bow)
                return true;

            return base.IsDodgeDamage();
        }

       

        protected override DamageAlgorithm GetDamageAlgorithm(GameClient target)
        {
            if (Spell != null && Spell.UseMana > 0)
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

        public override async Task Start()
        {
            if (Spell == null)
                return;

            // Delay skill cast
            await Task.Delay(Spell.DelayCast);

            int checkReq = CheckPrerequisites();
            if (CheckPrerequisites() != 0)
            {
                Console.WriteLine("Failed pre-requisites due to {0}", checkReq);
                return;
            }

            // find elligible targets to be hit
            if (CastX == 0 && CastY == 0 && Targets.Count > 0)
            {
                var firstTarget = Targets.First();
                CastX = firstTarget.X;
                CastY = firstTarget.Y;
            }
            FindTargets();

            // filter these targets
            //Console.WriteLine("Filter targets: {0}", Targets.Count);
            FilterTargets();

            // "If targets.Count > 0 or if the spell is a skill like fire circle, fb, ss, etc... then..."
            if (Targets.Count > 0 || Spell.Target == MagicTargetType.Ground)
            {
                // Consume
                ConsumePreRequisites();
            }

            // process doing the damage now to them
            //Console.WriteLine("Process targets: {0}", Targets.Count);
            ProcessTargets();
           
            // sync
            SynchronizeAll();
        }

        protected int CheckPrerequisites()
        {
            if (Spell == null)
                return 1;

            // if spell is xp, check for xp status
            if (Spell.IsUseXP && !Source.Status.IsAttached(StatusType.XpCircle))
                return 2; //fail if not xp bar

            if (!Spell.IsWeaponPassive)
            {
                // Set skill CD
                if (Source.NextMagic > DateTime.UtcNow)
                    return 3;
            }

            // stamina
            if (Source.Stamina < Spell.UseStamina)
                return 4;

            // mana
            if (Source.Mana < Spell.UseMana)
                return 5;

            return 0;
        }

        protected void ConsumePreRequisites()
        {
            // if all pre-reqs passed, update everything
            // no CD for passive skills
            if (!Spell.IsWeaponPassive)
                Source.NextMagic = DateTime.UtcNow.AddMilliseconds(Spell.DelayNextMagic);

            if (Spell.IsUseXP)
                Source.Status.Detach(StatusType.XpCircle);

            Source.Stamina -= Spell.UseStamina;
            Source.Mana -= Spell.UseMana;
        }

        protected override void SendDisplay(Dictionary<int, (int, bool)> power)
        {

            // send the damage packet to each target
            using (var p = new MagicEffectPacket()
                            .Begin(Source.Id, CastX, CastY, Spell.Type, Spell.Level, 0))
            {
                foreach (var kvp in power)
                    p.Add(kvp.Key, kvp.Value.Item1, kvp.Value.Item2 ? 0 : 1, 0);

                Source.FieldOfView.Send(p.End(), true);
            }

        }
    }
}
