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
        public GameClient Owner { get; private set; }
        public int Power { get; private set; }
        public DateTime? Expiration { get; private set; }
        public Status(GameClient owner)
        {
            Owner = owner;
        }
        public virtual void Attach(int power, TimeSpan? duration)
        {
            Power = power;
            Expiration = duration != null ? DateTime.UtcNow.Add((TimeSpan)duration) : DateTime.MaxValue;
        }
        public virtual void Detach()
        {
            Expiration = null;
        }
    }

    public class StatusManager
    {
        private Dictionary<StatusType, Status> _status;
        public GameClient Owner { get; private set; }

        public StatusManager(GameClient owner) 
        {
            Owner = owner;
            _status = new Dictionary<StatusType, Status>();
            _status[StatusType.Death] = new DeathStatus(owner);
            _status[StatusType.Fly] = new FlyStatus(owner);
        }

        public Status this[StatusType type]
        {
            get
            {
                return _status[type];
            }
        }

        public void AttachDeath(GameClient? source)
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

        public void Update()
        {
            foreach (StatusType type in _status.Keys)
            {
                Status status = _status[type];
                if (status.Expiration != null && DateTime.UtcNow >= status.Expiration)
                    status.Detach();
            }
        }

        public bool Has(StatusType type)
        {
            var status = _status[type];
            if (status.Expiration != null)
                return true;
            return false;
        }
        public int? GetPower(StatusType type)
        {
            var status = _status[type];
            if (status.Expiration != null)
                return status.Power;
            return null;
        }
    }
}
