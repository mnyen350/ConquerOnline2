using ConquerServer.Database.Models;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public class DeathStatus : Status
    {
        public GameClient? Source { get; private set; }

        public DeathStatus(GameClient owner)
            : base(owner)
        {

        }

        public void Attach(GameClient? source)
        {
            Source = source;
            this.Attach(0, null);
        }

        private void BroadcastDeath()
        {
            if (Source == null)
                return;

            int data0 = MathHelper.BitFold32(1, 0);
            using (InteractPacket ip = new InteractPacket(Source.Id, Owner.Id, Owner.X, Owner.Y, InteractAction.Kill, data0, 0, 0, 0))
                Source.FieldOfView.Send(ip, true);
        }

        public override void Attach(int power, TimeSpan? duration)
        {
            Owner.Health = 0;
            Owner.StatusFlag += StatusFlag.Death;

            // issue the death broadcast, and change to ghost
            Utility.Delay(TimeSpan.FromSeconds(0.15), async () =>
            {
                if (!Owner.IsDead) return; // revaldiate our assumption
                BroadcastDeath();

                await Task.Delay(TimeSpan.FromSeconds(1.5));

                if (!Owner.IsDead) return; // revaldiate our assumption
                Owner.StatusFlag += StatusFlag.Ghost;
                Owner.Lookface = Owner.Lookface.ToGhost();
                Owner.SendSynchronize(true);
                //Console.WriteLine($"{entity.Name} {entity.Lookface.ToUInt32()}");

                await Task.Delay(TimeSpan.FromSeconds(16.5));

                if (!Owner.IsDead) return;
                Owner.CanRevive = true;
            });

            base.Attach(power, duration);
        }

        public override void Detach()
        {
            //remove ghostface from lookface
            Owner.Lookface = Owner.Lookface.Normalize();

            Owner.StatusFlag -= StatusFlag.Ghost;
            Owner.StatusFlag -= StatusFlag.Death;

            //refill health
            Owner.Health = Owner.MaxHealth;

            //syncronize the stats with client
            Owner.SendSynchronize(true);

            base.Detach();
        }
    }
}
