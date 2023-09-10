using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Database
{
    public class DbException :Exception
    {
        public DbException(string type)
            : base(type)
        {

        }
    }
}
