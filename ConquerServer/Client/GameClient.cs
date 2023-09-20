using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network;
using ConquerServer.Network.Sockets;
using ConquerServer.Database;
using ConquerServer.Database.Models;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.CompilerServices;

namespace ConquerServer.Client
{
    public partial class GameClient : ILocation
    {
        private static NetworkDispatcher<GameClient> g_Network;
        private static SlashCommandDispatcher g_Slash;
        static GameClient()
        {
            g_Network = new NetworkDispatcher<GameClient>();
            g_Slash = new SlashCommandDispatcher();
        }


        private Dictionary<SynchronizeType, long> _sync;

        public ClientSocket Socket { get; private set; }
        public Db Database { get; private set; }
        public World World { get; private set; }
        public FieldOfView FieldOfView { get; private set; }
        public Inventory Inventory { get; private set; }
        public Equipment Equipment { get; private set; }
        public Dictionary<int, Magic> Magics { get; private set; }
        public string Username { get; private set; }
        public string Server { get; private set; }
        public int Id { get; set; }
        public uint Lookface { get; set; }
        public short HairStyle { get; set; }
        public int Gold { get; set; }
        public int ConquerPoints { get; set; }
        public long Experience { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Spirit { get; set; }
        public int AttributePoints { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public int PKPoints { get; set; }
        public int Level { get; set; }
        public int Job { get; set; }
        public int Rebirth { get; set; }
        public int QuizPoints { get; set; }
        public int EnlightenPoints { get; set; }
        public int VipLevel { get; set; }
        public int SubProfessionList { get; set; }
        public int Nationality { get; set; }
        public string Name { get; set; }
        public string SpouseName { get; set; }

        //map/location information 
        public int MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        //stats, all set in calcstats
        public int MinPhysicalAttack { get; private set; }
        public int MaxPhysicalAttack { get; private set; }
        public int MagicAttack { get; private set; }
        public int PhysicalDefense { get; private set; }
        public int MagicDefense { get; private set; }
        public int HitRate { get; private set; }
        public int Dodge { get; private set; }
        public Profession Profession
        {
            get
            {
                // is profession the CLASS name itself or the breakdown like "tiger trojan" or "dragon archer"
                // if its CLASS NAME... 
                int profession = (Job/10);
                if (profession > 10)
                    profession = 10;
                return (Profession)profession;
            }

        }
        public PKMode PKMode { get; private set; }

#warning, need a hostility/name flashing/name red whatever PKPOINTS 

        public GameClient(ClientSocket socket)
        {
            Socket = socket;
            Database = new Db(this);
            Server = string.Empty;
            Username = string.Empty;
            Name = string.Empty;
            SpouseName = string.Empty;
            World = new World(this);
            FieldOfView = new FieldOfView(this);
            Inventory = new Inventory(this);
            Equipment = new Equipment(this);
            Magics = new Dictionary<int, Magic>();
            _sync = new Dictionary<SynchronizeType, long>();
        }

        public bool DispatchNetwork(Packet msg)
        {
            var action = g_Network[msg.Type];
            if (action != null)
            {
                action(this, msg);
                return true;
            }
            return false;
        }
        public bool DispatchSlash(string[] messageContents)
        {
            //pull the method associated w the command/key within the dictionary
            var meth = g_Slash[messageContents[0].Substring(1)];
            if(meth != null)
            {
                //call the found method
                meth(this, messageContents);
                return true;
            }
            return false;
        }

        public void Disconnect()
        {
            Socket.Disconnect();
        }

        public void Send(Packet msg)
        {
            Socket.Send(msg);
        }

        public void SendSystemMessage(string message)
        {
            using (var systemMessage = new ChatPacket(ChatMode.System, Name, string.Empty, message))
            {
                this.Send(systemMessage);
            }
        }

         
        public void RecalculateStats()
        {
            // this method should recalculate stats based off of base values (i.e. items, base stats etc)

            // goal 1: calculate the max mana
            // by default the mana bonus is 5
            // however for taoists, it's a bit different
            // 100 -> 5 
            // 101 -> 10
            // 132 / 142 -> 15
            // 133 / 143 -> 20
            // 134 / 144 -> 25
            // 135 / 145 -> 30
            //
            // once the mana bonus is known,
            // max mana = (spirit * mana bonus) + equipment mana
            int manaBonus = 5;
            if (Profession == Profession.Taoist)
            {
                int rank = Math.Min(Job % 10, 5);
                manaBonus = manaBonus + (rank * manaBonus);
            }
#warning gear stat missing
            MaxMana = Spirit*manaBonus;

            // goal 2: calculate the max health
            // by default base stats increase hp by 3
            // however, vitality increases it by 24
            //
            // however for trojans, it's a bit different
            // they have a multiplier which gives them more hp...
            // 11 -> 1.05 
            // 12 -> 1.08 
            // 13 -> 1.10
            // 14 -> 1.12 
            // 15 -> 1.15
            //
            // max hp = [ ((str+spi+agi)*3) + (vit*24) ] * hp bonus
            double hpBonus = 1; 
            if(Profession == Profession.Trojan)
            {
                int rank = Math.Min(Job % 10, 5);
                if (rank == 1)
                    hpBonus = 1.05;
                else if (rank == 2)
                    hpBonus = 1.08;
                else if (rank == 3)
                    hpBonus = 1.10;
                else if (rank == 4)
                    hpBonus = 1.12;
                else if (rank ==5)
                    hpBonus = 1.15;
            }
            
            MaxHealth = Math.Max(1,(int)(((Strength + Spirit + Agility) * 3 + (Vitality) * 24) * hpBonus));
            Health = Math.Max(1, Math.Min(MaxHealth, Health));


            //set all stat properties
            MinPhysicalAttack = Equipment.MinPhysicalAttack;
            MaxPhysicalAttack = Equipment.MaxPhysicalAttack;
            MagicAttack = Equipment.MagicAttack;
            PhysicalDefense = Equipment.PhysicalDefense;
            MagicDefense = Equipment.MagicDefense;
            HitRate = (100 + Equipment.Dexterity) * Agility; // used in accuracy calculation
            Dodge = Equipment.Dodge;

            //send chatpacket to client for easy testing
            string dbgStr1 = string.Format(
                "P.Def: {0}, M. Def: {1}, P. Atk: {2} - {3}, M. Atk: {4}, HR: {5}, Dodge: {6}",
                 PhysicalDefense, MagicDefense, MinPhysicalAttack, MaxPhysicalAttack, MagicAttack, HitRate, Dodge);
            //string dbgStr2 = string.Format(
            //    "A. Mod: {0}, M. Mod: {1}, D. Mod: {2}, LS. Mod: {3}",
            //    AttackModifier, MAttackModifier, DefenseModifier, LifeStealModifier);

            this.SendChat(ChatMode.Action, dbgStr1);
            //this.Chat(ChatMode.Action, dbgStr2);


        }

        private void StartLoginSequence()
        {
            // is this character already logged in?
            var existingClient = World.Players.FirstOrDefault(g => g.Username == this.Username);
            if (existingClient != null)
            {
                // remove the player from the world
                World.RemovePlayer();
                // if so, then we need to disconnect
                existingClient.Disconnect();    
                // forcefully save it so that we load the most up to date character
                existingClient.Database.SaveCharacter();
            }
            
            
            //load the existing/just created char/file
            var lChar = Database.LoadCharacter();

            //in this instance of the game client, set the correct fields based off of lChar
            Id = lChar.Id;
            Lookface = lChar.Lookface;
            HairStyle = lChar.HairStyle;
            Gold = lChar.Gold;
            ConquerPoints = lChar.ConquerPoints;

            Experience = lChar.Experience;


            AttributePoints = lChar.AttributePoints;
            Health = lChar.Health;
            Mana = lChar.Mana;
            PKPoints = lChar.PKPoints;
            Level = lChar.Level;

            Job = lChar.Job;
            Rebirth = lChar.Rebirth;
            QuizPoints = lChar.QuizPoints;
            EnlightenPoints = lChar.EnlightenPoints;
            VipLevel = lChar.VipLevel;

            SubProfessionList = lChar.SubProfessionList;
            Nationality = lChar.Nationality;
            Name = lChar.Name;
            SpouseName = lChar.SpouseName;

            X = lChar.X;
            Y = lChar.Y;
            MapId = lChar.MapId;

            //Inventory
            foreach(var model in lChar.Inventory.Take(Inventory.MaxSize))
            {
                Item item = new Item(model, this);
                Inventory.TryAdd(item);
            }

            //Equipment
            foreach(var model in lChar.Equipment)
            {
                Item item = new Item(model, this);
                Equipment.Equip(item, item.Position, false);
            }
            
            //Magics
            foreach(var model in lChar.Magics)
            {
                //Magic magic = new Magic(this, model.TypeId, model.Level, model.Experience);
                //Magics.Add(model.TypeId, magic);
                LearnMagic(model.TypeId, model.Level, model.Experience);
            }
                

            if (lChar.Rebirth > 0)
            {
                Strength = lChar.Strength;
                Agility = lChar.Agility;
                Vitality = lChar.Vitality;
                Spirit = lChar.Spirit;
            }
            else
            {
                StatModel stat =  Database.GetStats(Profession, Level);
                Strength = stat.Strength;
                Agility = stat.Agility;
                Vitality = stat.Vitality;
                Spirit = stat.Spirit;
            }

            Equipment.Update(); // this will recalculate stats too

            World.AddPlayer();

            SendChat(ChatMode.Entrance, "ANSWER_OK");
            SendMapConfirmation();
            SendServerTime();
            SendHeroInformation();
            FieldOfView.Move(this.MapId, this.X, this.Y); // update FOV
        }

        public void Teleport(int x, int y) => Teleport(this.MapId, x, y);

        public void Teleport(int mapId, int x, int y)
        {
            using (var p = CreateAction(Id, x, y, 0, ActionType.Teleport, mapId))
                FieldOfView.Send(p, true);
            FieldOfView.Clear();
            FieldOfView.Move(mapId, x, y);
        }

        
        public void LearnMagic(int typeId, int level=0, long experience =0) 
        {
            Magic magic = new Magic(this, typeId, level, experience);
            Magics[typeId] = magic; // add or update

            //send to player, the learned magic/skill

            using (var p = new Packet(32))
            {
                p.WriteUInt32(TimeStamp.GetTime()); // 5735
                p.WriteUInt32((uint)experience);
                p.WriteUInt16((ushort)typeId);
                p.WriteUInt16((ushort)level);
                p.WriteInt16((short)0);
                p.WriteInt16((byte)0);
                p.WriteInt32(0);
                p.Build(PacketType.MagicInfo);
                Send(p);
            }
        }

        private Dictionary<SynchronizeType, long> CreateSynchronize()
        {
            return new Dictionary<SynchronizeType, long>
            {
                { SynchronizeType.Health, Health },
                { SynchronizeType.MaxLife, MaxHealth },
                { SynchronizeType.Mana, Mana },
                { SynchronizeType.MaxMana, MaxMana },
                { SynchronizeType.Stamina, 100 }
            };
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

            // make the packet and send it
            if (diff.Count > 0)
            {
                using (var p = new SynchronizePacket()
                                      .Begin(this.Id))
                {
                    foreach (var kvp in diff)
                        p.Synchronize(kvp.Key, kvp.Value);

                    p.End();

                    this.Send(p);
                }

                // update old sync
                _sync = newSync;
            }
        }


        public void SendChat(ChatMode mode, string words)
        {
            using (var msg = new ChatPacket(mode, words))
            {
                Send(msg);
            }
        }

        public void SendMapConfirmation()
        {
            using (var p = new Packet(20))
            {
                p.WriteUInt32(TimeStamp.GetTime()); // 5735
                p.WriteInt32(0);
                p.WriteInt32(1);
                p.WriteInt32(MapId);
                p.Build(PacketType.LoadMap);
                Send(p);
            }
        }

        public void SendServerTime()
        {
            DateTime time = DateTime.Now; // get local time
            using (var p = new Packet(40))
            {

                p.WriteUInt32(TimeStamp.GetTime()); // 5735
                p.WriteInt32(0); // type
                p.WriteInt32(time.Year - 1900);
                p.WriteInt32(time.Month - 1);
                p.WriteInt32(time.DayOfYear);
                p.WriteInt32(time.Day);
                p.WriteInt32(time.Hour);
                p.WriteInt32(time.Minute);
                p.WriteInt32(time.Second);
                p.Build(PacketType.Data);
                Send(p);
            }
        }

        
        public void SendHeroInformation()
        {
            using (var p = new Packet(PacketBufferSize.SizeOf512))
            {
                p.WriteUInt32(TimeStamp.GetTime()); // 5735
                p.WriteInt32(Id);
                p.WriteInt16(0); //AppearanceType
                p.WriteUInt32(Lookface); //AppearanceId
                p.WriteInt16(HairStyle); //HairStyle
                p.WriteInt32(Gold);
                p.WriteInt32(ConquerPoints);
                p.WriteInt64(Experience);
                p.Fill(5 * sizeof(int)); // "deed", "medal", "medal_select", "virtue", "mete_lev"
                p.WriteUInt16((ushort)Math.Min(Strength, 65535));
                p.WriteUInt16((ushort)Math.Min(Agility, 65535));
                p.WriteUInt16((ushort)Math.Min(Vitality, 65535));
                p.WriteUInt16((ushort)Math.Min(Spirit, 65535));
                p.WriteUInt16((ushort)Math.Min(AttributePoints, 65535));
                p.WriteInt32(Health);
                p.WriteUInt16((ushort)Math.Min(Mana, 65535));
                p.WriteUInt16((ushort)Math.Min(PKPoints, 65535));
                p.WriteInt8((byte)Math.Min(Level, 255));
                p.WriteInt8((byte)Math.Min(Job, 255));
                p.WriteInt8(0); // birth profession
                p.WriteInt8(0); // first rebirth profession
                p.Fill(1);
                p.WriteInt8((byte)Math.Min(Rebirth, 255));
                p.WriteInt8(0); // unknown
                p.WriteInt32(QuizPoints); // quiz point
                p.WriteInt32(0); // privilege (added 5728)
                p.WriteInt16((short)EnlightenPoints); // enlighten point
                p.WriteInt16(0); // unknown
                p.Fill(2);
                p.WriteInt16(0); // merit point- not used
                p.WriteInt32(VipLevel); // vip level
                p.WriteInt16(0); // title
                p.WriteInt32(0); //Bound CPs - fuck bound CPs
                p.WriteInt8(0); // selected sub profession (not used anymore?)
                p.WriteInt64(SubProfessionList); // sub profession info
                p.WriteInt32(0); // race point
                p.WriteInt16((short)Nationality);

                p.WriteStrings(Name, string.Empty, SpouseName ?? "None");
                p.Build(PacketType.UserInfo);
                Send(p);
            }
        }
        #endregion


        #region CREATE methods
        public void SendItemUse(ItemAction action, int id, uint timeStamp = 0, params int[] args)
        {
            if (timeStamp == 0)
                timeStamp = TimeStamp.GetTime();

            using (var p = CreateItemUse(action, id, timeStamp, args))
                Send(p);
        }

        public void SendItemInfo(Item item, ItemInfoAction action, bool extended = false)
        {
            using (Packet msg = new Packet(128))
            {
                msg.WriteInt32(item.Id); // id
                if (extended)
                {
                    msg.WriteInt32(item.OwnerId);
                    msg.WriteInt32(item.Cost);
                }
                msg.WriteInt32(item.TypeId); // itemtype id
                msg.WriteInt16((short)item.Durability); // amount/durability
                msg.WriteInt16((short)item.MaxDurability); // amount/durability limit
                msg.WriteInt8((byte)action); // action
                msg.WriteInt8(0); // ident
                msg.WriteInt8((byte)item.Position); // position
                msg.Fill(1);
                msg.WriteInt32(item.AltProgress); // "data" (used with talisman progress and steed color?
                msg.WriteInt8((byte)item.Gem1); // gem1
                msg.WriteInt8((byte)item.Gem2); // gem2

                // -- 7 --
                msg.WriteInt16((short)0); // padding
                msg.WriteInt32(item.WeaponAttribute); // magic1 (weapon magic)
                msg.WriteInt8((byte)0); // magic2 (unused)

                msg.WriteInt8((byte)item.Composition); // magic3 (composition)
                msg.WriteInt8((byte)item.Bless); // reduce dmg (bless)
                msg.WriteInt8((byte)item.LockStatus); // free (0/1/2/3)
                msg.WriteInt8((byte)item.Enchant); // add life (enchant)

                // -- 11 ---
                msg.Fill(11);

                msg.WriteInt32(item.Color); // color
                msg.WriteInt32(item.CompositionProgress); // add level exp (for "new" composition system)
                msg.WriteInt32(0); // inscribed (arsenals)
                msg.WriteInt32((int)(item.Duration.HasValue ? item.Duration.Value.TotalSeconds : 0)); //Expire time
                msg.WriteInt32(0);
                msg.WriteInt32(item.Stack); // stack (defaults to 1 it seems)
                if (extended)
                {
                    msg.WriteInt32(0);
                    msg.WriteInt32(0); // dragon soul id
                    msg.WriteInt32(0);
                }

                msg.Build(extended ? PacketType.ItemInfoEx : PacketType.ItemInfo);
                Send(msg);
            }

            //if (item.DragonSoulId != 0)
            //{
            //    using (var p = CreateItemStatus(item))
            //        Send(p);
            //}
        }
        public static Packet CreateItemUse(ItemAction action, int id, uint ts, params int[] args)
        {
            var p = new Packet(128);
            p.WriteUInt32(TimeStamp.GetTime()); // 5735
            p.WriteInt32(id);
            p.WriteInt32(args.Length > 0 ? args[0] : 0);
            p.WriteInt32((int)action);
            p.WriteUInt32(ts);
            for (int i = 1; i < args.Length; i++)
                p.WriteInt32(args[i]);
            p.Build(PacketType.Item);
            return p;
        }

        public static Packet CreateAction(int id, int posX, int posY, int dir, ActionType mode, long data)
        {
            var p = new Packet(64);

            p.WriteUInt32(TimeStamp.GetTime()); // 4
            p.WriteInt32(id);                   // 8
            p.WriteInt64(data);                 // 12
            p.WriteUInt32(TimeStamp.GetTime()); // 20
            p.WriteInt16((short)mode);          // 24
            p.WriteInt16((short)dir);           // 26
            p.WriteInt16((short)posX);          // 28
            p.WriteInt16((short)posY);          // 30
            p.Fill(sizeof(int));                // 32
            p.Fill(sizeof(int));                // 36
            p.Fill(sizeof(byte));               // 40
            p.Fill(sizeof(byte));               // 41
            p.Build(PacketType.Action);

            return p;
        }
        public static Packet CreateEntityPacket(GameClient e)
        {
            var msg = new Packet(PacketBufferSize.SizeOf512);
            msg.WriteUInt32(TimeStamp.GetTime()); // 5735
            msg.WriteUInt32(e.Lookface);
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

            if (e.Id >= 1000000) //
            {
                msg.WriteInt32(e.Equipment[ItemPosition.Set1Helmet]?.TypeId ?? 0);
                msg.WriteInt32(e.Equipment[ItemPosition.Set1Garment]?.TypeId ?? 0);
                msg.WriteInt32(e.Equipment[ItemPosition.Set1Armor]?.TypeId ?? 0);

                msg.WriteInt32(e.Equipment[ItemPosition.Set1Weapon1]?.TypeId ?? 0);// e.WeaponLeftTypeId);
                msg.WriteInt32(e.Equipment[ItemPosition.Set1Weapon2]?.TypeId ?? 0); // e.WeaponRightTypeId);
                msg.WriteInt32(e.Equipment[ItemPosition.W1Accessory]?.TypeId ?? 0); // e.WeaponLeftCoatTypeId);
                msg.WriteInt32(e.Equipment[ItemPosition.W2Accessory]?.TypeId ?? 0); // e.WeaponRightCoatTypeId);


                msg.WriteInt32(e.Equipment[ItemPosition.Steed]?.TypeId ?? 0); //MountTypeId
                msg.WriteInt32(e.Equipment[ItemPosition.SteedAccessory]?.TypeId ?? 0); //MountDecoratorTypeId
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

            ///*if (e.Lookface.Mesh <= 4)
            //{
            //    msg.WriteInt32(e.HelmetTypeId);
            //    msg.WriteInt32(e.OvercoatTypeId);
            //    msg.WriteInt32(e.ArmorTypeId);
            //}
            //else*/
            //{
            //    msg.WriteInt32(0);
            //    msg.WriteInt32(0);
            //    msg.WriteInt32(0);
            //}
            //msg.WriteInt32(0);// e.WeaponLeftTypeId);
            //msg.WriteInt32(0); // e.WeaponRightTypeId);
            //msg.WriteInt32(0); // e.WeaponLeftCoatTypeId);
            //msg.WriteInt32(0); // e.WeaponRightCoatTypeId);
            ///*if (e.Lookface.Mesh <= 4)
            //{
            //    msg.WriteInt32(e.MountTypeId);
            //    msg.WriteInt32(e.MountDecoratorTypeId);
            //}
            //else*/
            //{
            //    msg.WriteInt32(0);
            //    msg.WriteInt32(0);
            //}


            msg.WriteInt16(0); // unknown
            msg.WriteInt16(0); // unknown
            msg.WriteInt16(0); // speed percent?
            msg.WriteInt32(e.Health); // monster life
            msg.WriteInt16(0); // selected medal
            if (e.Id < 1000000)
                msg.WriteInt16((short)e.Level); // monster level
            else
                msg.WriteInt16(0);
            //msg.WriteUInt32(e.Position.Point); // pos x/y
            msg.WriteInt16((short)e.X);
            msg.WriteInt16((short)e.Y);
            msg.WriteInt16((short)e.HairStyle); // hair
            msg.WriteInt8((byte)0); // e.Direction); // direction
            msg.WriteInt32((int)0); // e.Stance); // pose
            msg.WriteInt16(0); // continue action
            msg.Fill(1);
            msg.WriteInt8((byte)e.Rebirth); // metempsychosis
            if (e.Id >= 1000000)
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

            if (e.Id >= 1000000) //player
            {
                int h = e.Equipment[ItemPosition.Set1Helmet]?.Color ?? 0;
                msg.WriteInt16((short)(e.Equipment[ItemPosition.Set1Armor]?.Color ?? 0)); // e.ArmorColor); // armor color
                msg.WriteInt16((short)(e.Equipment[ItemPosition.Set1Weapon2]?.Color ?? 0)); // e.ShieldColor); // shield color
                msg.WriteInt16((short)(h)); // e.HelmetColor); // helmet color
            }
            else //monsters
            {
                msg.WriteInt16((short)0); 
                msg.WriteInt16((short)0); 
                msg.WriteInt16((short)0); 
            }

            msg.WriteInt32(e.QuizPoints); // quiz points
            msg.WriteInt16((short)0); // e.MountAdd); // mount add
            msg.WriteInt32(0); // mount exp
            msg.WriteInt32(0); // e.MountColor); // mount color
            msg.WriteInt16((short)e.EnlightenPoints); // enlighten point
            msg.WriteInt16(0); // merit point
            msg.WriteInt16(0); // unknown
            msg.WriteInt16((short)0); // e.EnlightenDayInfo); // coach day info
            msg.WriteInt32(e.VipLevel); // vip level
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
            msg.WriteInt64(e.SubProfessionList); // subprofession info
            msg.WriteInt16((short)0); // e.FirstJob); // birth profession
            msg.WriteInt16(0); // first rebirth profession
            msg.WriteInt16((short)e.Job); // profession
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
            //if (e.Event != null)
            //{
            //    clan = e.Event.Name;
            //}

            msg.WriteStrings(e.Name, mate, clan, string.Empty, string.Empty, string.Empty); // name, mate, clan

            // extended info is always supported
            //msg.WriteInt8((byte)e.SizeAdd);
            ///msg.WriteInt8((byte)(e.IsBoss ? 1 : 0));
            //msg.WriteInt16((short)e.ZoomPercent);
            //msg.WriteInt32(e.MaxHealth);

            msg.Build(PacketType.SpawnEntity);
            // Log.Write("{0}", msg.Dump());
            return msg;
        }

        public static Packet CreateDespawnPacket(GameClient e)
        {
            return CreateAction(e.Id, 0, 0, 0, ActionType.RemoveEntity, 0);
        }

        #endregion
    }
}
