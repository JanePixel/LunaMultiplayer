using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.BaseStore
{
    // Store data and states used in JPCC not related to config variables
    public class BaseKeeper
    {
        public BaseKeeper() {}

        public string Version = "";

        public string About = "";

        public bool ResetWorld = false;
    }
}
