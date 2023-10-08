using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Shared;

namespace ConquerServer.Database.Models
{
    public class PortalModel : ILocation
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int MapId { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
        public int ToMapId { get; set; }

        public PortalModel()
        {

        }

        public static PortalModel Parse(string s)
        {
            int[] portalData = s
                    .Split(" ")
                    .Select(s => int.Parse(s))
                    .ToArray();

            PortalModel portal = new PortalModel()
            {
                X = (portalData[0]),
                Y = (portalData[1]),
                MapId = (portalData[2]),
                ToX = (portalData[3]),
                ToY = (portalData[4]),
                ToMapId = (portalData[5])
            };
            return portal;
        }
    }

}
