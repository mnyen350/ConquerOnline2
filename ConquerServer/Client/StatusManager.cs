using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public abstract class Status
    {
        public Entity Owner { get; private set; }
        public int Power { get; private set; }
        public DateTime? Expiration { get; private set; }
        public Status(Entity owner)
        {
            Owner = owner;
        }
        public virtual void Attach(int power, TimeSpan? duration)
        {
            Power = power;
            Expiration = duration != null ? DateTime.UtcNow.Add((TimeSpan)duration) : DateTime.MaxValue;
            Owner.SendSynchronize();
        }
        public virtual void Detach()
        {
            if (Expiration != null)
            {
                Power = 0;
                Expiration = null;
                Owner.SendSynchronize();
            }
        }
    }

    public class StatusManager
    {
        private Dictionary<StatusType, Status> _status;
        public Entity Owner { get; private set; }

        public StatusManager(Entity owner) 
        {
            Owner = owner;
            _status = new Dictionary<StatusType, Status>();
            _status[StatusType.Death] = new DeathStatus(owner);
            _status[StatusType.Fly] = new FlyStatus(owner);
            _status[StatusType.Defense] = new DefenseStatus(owner); //water tao shield and warrior Xp shield 
            _status[StatusType.Attack] = new AttackStatus(owner);
            _status[StatusType.XpCircle] = new XpCircleStatus(owner);
            _status[StatusType.Superman] = new SupermanStatus(owner);
            _status[StatusType.Cyclone] = new CycloneStatus(owner);
            _status[StatusType.XPDefense] = new DefenseStatus(owner);
            _status[StatusType.YinYang] = new YinYangStatus(owner);
            _status[StatusType.Hitrate] = new HitRateStatus(owner);
            _status[StatusType.Intensify] = new IntensifyStatus(owner);
        }

        public Status this[StatusType type]
        {
            get
            {
                return _status[type];
            }
        }

        public void AttachDeath(Entity? source)
        {
            var deathStatus = (DeathStatus)_status[StatusType.Death];
            deathStatus.Attach(source);
        }

        public void Attach(StatusType type, int power=0, TimeSpan? duration = null)
        {
            var status = _status[type];
            status.Attach(power, duration);
        }

        public void Detach(params StatusType[] types)
        {
            foreach (var effect in types)
                _status[effect].Detach();
        }

        public void DetachExpired()
        {
            foreach (StatusType type in _status.Keys)
            {
                Status status = _status[type];
                if (status.Expiration != null && DateTime.UtcNow >= status.Expiration)
                    status.Detach();
            }
        }

        public bool IsAttached(StatusType type)
        {
            var status = _status[type];
            if (status.Expiration != null)
                return true;
            return false;
        }
        public int GetPower(StatusType type, int defaultValue = 0)
        {
            var status = _status[type];
            if (status.Expiration != null)
                return status.Power;
            return defaultValue;
        }
    }
}
