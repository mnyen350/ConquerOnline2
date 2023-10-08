using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Client;

namespace ConquerServer.Database.Models
{
    public abstract class SkillModel
    {
        public int OwnerId { get; set; }
        public long Experience { get; set; }
        public int TypeId { get; set; }
        public int Level { get; set; }
    }

    public class MagicModel : SkillModel
    {
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

    public class ProficiencyModel : SkillModel
    {
        public ProficiencyModel()
        {

        }

        public ProficiencyModel(Proficiency prof)
        {
            OwnerId = prof.Owner.Id;
            Experience = prof.Experience;
            TypeId = prof.TypeId;
            Level = prof.Level;
        }
    }
}
