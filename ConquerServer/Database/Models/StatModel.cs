using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database.Models
{
    public class StatModel
    {
        public int Strength { get; private set; }
        public int Agility { get; private set; }
        public int Vitality { get; private set; }
        public int Spirit { get; private set; }

        public StatModel(int strength, int agility, int vitality, int spirit)
        {
            Strength = strength;
            Agility = agility;
            Vitality = vitality;
            Spirit = spirit;
        } 
    }
}
