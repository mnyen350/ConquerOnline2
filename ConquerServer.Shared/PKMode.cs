using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Shared
{
    public enum PKMode
    {
        Guild = 6, ///hit everyone not in team, guild and allies
        Kill = 0, //hit everyone
        Team = 2, //hit everyone not in team
        /// <summary>
        /// Cannot hit anyone
        /// </summary>
        Peace = 1, //hit no one
        Capture =3, //hit flashing/red/black
        Revenge =4 //hit everyone on your enemy list
    }
}
