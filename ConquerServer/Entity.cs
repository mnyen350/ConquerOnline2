using ConquerServer.Client;
using ConquerServer.Network;
using ConquerServer.Network.Packets;
using ConquerServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer
{
    public class Entity : ILocation
    {
        protected const int MAX_STAMINA = 100;
        protected const int MAX_XP = 100;
        public virtual bool IsPlayer => false;
        private Dictionary<SynchronizeType, long> _sync;
        public int Id { get; set; }
        public string Name { get; set; }
        public World World { get; private set; }
        public FieldOfView FieldOfView { get; private set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public DateTime NextMagic { get; set; }
        public PKMode PKMode { get; set; }
        public EmoteType Emote { get; set; }

        public StatusManager Status { get; set; }
        public StatusFlag StatusFlag { get; set; }
        public Lookface Lookface { get; set; }

        //stats
        public bool IsDead { get { return (Health <= 0); } }
        private int _health;
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = MathHelper.Clamp(value, 0, MaxHealth);
            }
        }
        public int MaxHealth { get; set; }
        private int _mana;
        public int Mana
        {
            get
            {
                return _mana;
            }
            set
            {
                _mana = MathHelper.Clamp(value, 0, MaxMana);
            }
        }
        public int MaxMana { get; set; }
        private int _stamina;
        public int Stamina
        {
            get { return _stamina; }
            set { _stamina = MathHelper.Clamp(value, 0, MAX_STAMINA); }
        }

        //map/location information 
        public int MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }


        //stats, all set in calcstats
        public int MinPhysicalAttack { get; protected set; }
        public int MaxPhysicalAttack { get; protected set; }
        public int MagicAttack { get; protected set; }
        public int PhysicalDefense { get; protected set; }
        public int MagicDefense { get; protected set; }
        public int HitRate { get; protected set; }
        public int Dodge { get; protected set; }

        private int _xp;
        public int Xp
        {
            get { return _xp; }
            set { _xp = MathHelper.Clamp(value, 0, MAX_XP); }
        }
        public Entity()
        {
            Name = string.Empty;
            World = new World(this);
            Lookface = new Lookface();
            Status = new StatusManager(this);
            StatusFlag = StatusFlag.None;
            _sync = new Dictionary<SynchronizeType, long>();
            //change later
            NextMagic = DateTime.MinValue;
            FieldOfView = new FieldOfView(this);
        }

        public void Teleport(int x, int y) => Teleport(this.MapId, x, y);

        public virtual void Teleport(int mapId, int x, int y)
        {
            FieldOfView.Clear();
            FieldOfView.Move(mapId, x, y);
        }
        #region SEND methods
        public void SendSynchronize()
        {
            var newSync = CreateSynchronize();
            var diff = new Dictionary<SynchronizeType, long>();

            // find the differences between _sync and newSync
            foreach (var kvp in newSync)
            {
                long oldValue;
                if (!_sync.TryGetValue(kvp.Key, out oldValue) || oldValue != kvp.Value)
                    diff[kvp.Key] = kvp.Value;
            }

            if (diff.Count > 0)
            {
                bool broadcast = diff.Keys.Any(type => IsBroadcastSynchronizeType(type));

                // make the packet and send it
                using (var p = new SynchronizePacket()
                                      .Begin(this.Id))
                {
                    // sync any differences
                    foreach (var kvp in diff)
                    {
                        // when flags, kvp value is the hash, so we sync the actual value
                        if (kvp.Key == SynchronizeType.Flags)
                            p.Synchronize(SynchronizeType.Flags, this.StatusFlag.Bits);
                        else
                            p.Synchronize(kvp.Key, kvp.Value);
                    }

                    p.End();

                    if (broadcast) this.FieldOfView.Send(p, true);
                    else this.Send(p);
                }
            }

            // update old sync
            _sync = newSync;
        }
        private Dictionary<SynchronizeType, long> CreateSynchronize()
        {
            return new Dictionary<SynchronizeType, long>
            {
                { SynchronizeType.Health, Health },
                { SynchronizeType.MaxLife, MaxHealth },
                { SynchronizeType.Mana, Mana },
                { SynchronizeType.MaxMana, MaxMana },
                { SynchronizeType.Stamina, Stamina },
                { SynchronizeType.Lookface, (long)Lookface },
                { SynchronizeType.XPCircle, Xp},
                { SynchronizeType.Flags, StatusFlag.GetHashCode() }
            };
        }
        private bool IsBroadcastSynchronizeType(SynchronizeType type)
        {
            switch (type)
            {
                case SynchronizeType.Hair:
                case SynchronizeType.MetempsychosisLevel:
                case SynchronizeType.Metempsychosis:
                case SynchronizeType.Lookface:
                case SynchronizeType.Flags: return true;
                default: return false;
            }
        }
        public virtual void Send(Packet msg)
        {
            
        }
        #endregion

        #region CREATE methods
        public static Packet CreateEntityPacket(Entity e)
        {
            GameClient? ep = e.IsPlayer ? (GameClient)e : null;

            var msg = new Packet(PacketBufferSize.SizeOf512);
            msg.WriteUInt32(TimeStamp.GetTime()); // 5735
            msg.WriteUInt32((uint)e.Lookface);
            msg.WriteInt32(e.Id);
            msg.WriteInt32(0); // e.GuildId
            msg.WriteInt32(0); // e.GuildRank
            msg.WriteInt16(0); // e.GuildTitle);
            //foreach (var bits in e.Flags.Bits)
            //{
            //    msg.WriteInt32(bits);
            //}
            msg.WriteInt32(0);
            msg.WriteInt32(0);
            msg.WriteInt32(0);
            msg.WriteInt32(0);
            msg.WriteInt32(0);
            msg.WriteInt16(0); // appearance type

            if (e.IsPlayer) //
            {
                msg.WriteInt32(ep.Equipment[ItemPosition.Set1Helmet]?.TypeId ?? 0);
                msg.WriteInt32(ep.Equipment[ItemPosition.Set1Garment]?.TypeId ?? 0);
                msg.WriteInt32(ep.Equipment[ItemPosition.Set1Armor]?.TypeId ?? 0);

                msg.WriteInt32(ep.Equipment[ItemPosition.Set1Weapon2]?.TypeId ?? 0); // e.WeaponRightTypeId);
                msg.WriteInt32(ep.Equipment[ItemPosition.Set1Weapon1]?.TypeId ?? 0);// e.WeaponLeftTypeId);
                msg.WriteInt32(ep.Equipment[ItemPosition.W1Accessory]?.TypeId ?? 0); // e.WeaponLeftCoatTypeId);
                msg.WriteInt32(ep.Equipment[ItemPosition.W2Accessory]?.TypeId ?? 0); // e.WeaponRightCoatTypeId);


                msg.WriteInt32(ep.Equipment[ItemPosition.Steed]?.TypeId ?? 0); //MountTypeId
                msg.WriteInt32(ep.Equipment[ItemPosition.SteedAccessory]?.TypeId ?? 0); //MountDecoratorTypeId
            }
            else //monsters
            {
                msg.WriteInt32(0);
                msg.WriteInt32(0);
                msg.WriteInt32(0);

                msg.WriteInt32(0);
                msg.WriteInt32(0);
                msg.WriteInt32(0);
                msg.WriteInt32(0);

                msg.WriteInt32(0);
                msg.WriteInt32(0);
            }

            msg.WriteInt16(0); // unknown
            msg.WriteInt16(0); // unknown
            msg.WriteInt16(0); // speed percent?
            msg.WriteInt32(e.Health); // monster life
            msg.WriteInt16(0); // selected medal
            if (e.IsPlayer)
                msg.WriteInt16((short)e.Level); // monster level
            else
                msg.WriteInt16(0);
            //msg.WriteUInt32(e.Position.Point); // pos x/y
            msg.WriteInt16((short)e.X);
            msg.WriteInt16((short)e.Y);
            msg.WriteInt16((short)(ep?.HairStyle ?? 0)); // hair
            msg.WriteInt8((byte)0); // e.Direction); // direction
            msg.WriteInt32((int)0); // e.Stance); // pose
            msg.WriteInt16(0); // continue action
            msg.Fill(1);
            msg.WriteInt8((byte)(ep?.Rebirth ?? 0)); // metempsychosis
            if (!e.IsPlayer)
                msg.WriteInt16((short)e.Level); // monster level
            else
                msg.WriteInt16(0);
            msg.WriteInt8((byte)0); // (view ? 1 : 0)); // lock dummy
            msg.WriteInt8((byte)0); // (e.IsAway ? 1 : 0)); // away status
            msg.WriteInt32(0); // tutor battle effect
            msg.WriteInt32(0); // chi battle effect
            msg.WriteInt32(0); // team amount
            msg.WriteInt32(0); // team leader id
            msg.WriteInt32(0); // flowers
            msg.WriteInt32(0); // nobility

            if (e.IsPlayer) //player
            {
                msg.WriteInt16((short)(ep.Equipment[ItemPosition.Set1Armor]?.Color ?? 0)); // e.ArmorColor); // armor color
                msg.WriteInt16((short)(ep.Equipment[ItemPosition.Set1Weapon2]?.Color ?? 0)); // e.ShieldColor); // shield color
                msg.WriteInt16((short)(ep.Equipment[ItemPosition.Set1Helmet]?.Color ?? 0)); // e.HelmetColor); // helmet color
            }
            else //monsters
            {
                msg.WriteInt16((short)0);
                msg.WriteInt16((short)0);
                msg.WriteInt16((short)0);
            }

            msg.WriteInt32(ep?.QuizPoints ?? 0); // quiz points
            msg.WriteInt16((short)0); // e.MountAdd); // mount add
            msg.WriteInt32(0); // mount exp
            msg.WriteInt32(0); // e.MountColor); // mount color
            msg.WriteInt16((short)(ep?.EnlightenPoints ?? 0)); // enlighten point
            msg.WriteInt16(0); // merit point
            msg.WriteInt16(0); // unknown
            msg.WriteInt16((short)0); // e.EnlightenDayInfo); // coach day info
            msg.WriteInt32(ep?.VipLevel ?? 0); // vip level
            msg.WriteInt32(0); // e.Event != null ? e.Event.Id : 0); // clan id
            msg.WriteInt32((int)0); // (e.Event != null ? (e.Event.Id == e.Id) ? ClanRank.Leader : ClanRank.Member : 0)); // clan rank
            msg.WriteInt32(0); // clan battle effect
            msg.WriteInt16(0); // title
            msg.WriteInt32(0); // e.SpeedPercent); // custom -- 0xb7; speed percent
            msg.WriteInt8(0); // texas actor
            msg.WriteInt32(0); // arsenal battle effect
            msg.WriteInt8(0); // arena witness
            msg.WriteInt8((byte)0); // e.RoleType); // unknown #custom -- interact type
            msg.WriteInt8(0); // unknown
            msg.WriteInt8((byte)0); // (e.IsBoss ? 1 : 0)); // boss
            msg.WriteInt32(0); // e.HelmetSoulId); // helm art id
            msg.WriteInt32(0); // e.ArmorSoulId); // armr art id
            msg.WriteInt32(0); // e.WeaponLeftSoulId); // wep2 art id
            msg.WriteInt32(0); // e.WeaponRightSoulId); // wep1 art id
            msg.WriteInt8(0); // selected subprofession
            msg.WriteInt64(ep?.SubProfessionList  ?? 0); // subprofession info
            msg.WriteInt16((short)0); // e.FirstJob); // birth profession
            msg.WriteInt16(0); // first rebirth profession
            msg.WriteInt16((short) (ep?.Job ?? 0)); // profession
            msg.WriteInt16((short)0); // e.Nationality); // country code
            msg.WriteInt32(0); // team id
            msg.WriteInt32(0); // e.BattlePower); // battle effect
            msg.WriteInt8(0); // gang hood level (jiang hu)
            msg.WriteInt8(0); // gang hood tag (jiang hu)
            msg.WriteInt8(0); // unknown (used)

            //msg.WriteInt16(0); // server name (5929)
            //msg.WriteInt8(0); // call pet type (5936)
            //msg.WriteInt16(0); // attack range (5936)
            //msg.WriteInt32(0); // owner id (5936)

            string mate = "";
            string clan = "";

            msg.WriteStrings(e.Name, mate, clan, string.Empty, string.Empty, string.Empty); // name, mate, clan

            msg.Build(PacketType.SpawnEntity);
            // Log.Write("{0}", msg.Dump());
            return msg;
        }
        public static Packet CreateDespawnPacket(Entity e)
        {
            return new ActionPacket(e.Id, 0, 0, 0, ActionType.RemoveEntity, 0);
        }
        #endregion

        public override int GetHashCode() => Id;
    }
}
