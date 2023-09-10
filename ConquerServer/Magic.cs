using ConquerServer.Client;
using ConquerServer.Database;
using ConquerServer.Database.Models;
using ConquerServer.Network;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConquerServer
{
    public class Magic
    {
        public GameClient Owner { get; set; }
        public long Experience { get; set; }

        public int TypeId { get; set; }
        public int Level { get; set; }

        private MagicTypeModel? _attributes;

        public MagicTypeModel Attributes
        {
            get
            {
                //typeid + level for composite key
                if(_attributes == null || _attributes.Type == this.TypeId || _attributes.Level == this.Level)
                {
                    _attributes = Db.GetMagicType(this.TypeId, this.Level);
                }
                return _attributes;
            }
        }

        public Magic(GameClient client, int typeId, int level =0, long experience = 0)
        {
            Owner = client;
            Experience = experience;
            TypeId = typeId;
            Level = level;
        }
    }
}
