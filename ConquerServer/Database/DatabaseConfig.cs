using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database
{
    public class DatabaseConfig
    {
        public string ServerHost { get; set; }
        public int CharacterCounter { get; set; }
        public int ItemCounter { get; set; }

        public DatabaseConfig()
        {
            ServerHost = "192.168.0.119";
            CharacterCounter = 1000000;
            ItemCounter = 2000000;
        }
    }
}
