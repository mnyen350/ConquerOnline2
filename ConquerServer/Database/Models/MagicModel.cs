using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database.Models
{
    public class MagicModel
    {
        public int OwnerId { get; set; }
        public long Experience { get; set; }
        public int TypeId { get; set; }
        public int Level { get; set; }

        public MagicModel()
        {

        }
        public MagicModel(Magic magic)
        {
            OwnerId = magic.Owner.Id; 
            Experience = magic.Experience;
            TypeId = magic.TypeId;
            Level = magic.Level;
        }
    }
}
