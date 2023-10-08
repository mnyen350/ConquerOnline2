using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database.Models
{
    public class RevivePointModel
    {
        public int DeathMapId { get; set; }
        public int ReviveMapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public RevivePointModel() 
        {
            
        }

        public static RevivePointModel Parse(string input)
        {
            RevivePointModel rpm = new RevivePointModel();
            string[] split = input.Split(' ');
            rpm.DeathMapId = int.Parse(split[0]);
            rpm.ReviveMapId= int.Parse(split[1]);
            rpm.X = int.Parse(split[2]);
            rpm.Y = int.Parse(split[3]);
            return rpm;
        }
    }
}
