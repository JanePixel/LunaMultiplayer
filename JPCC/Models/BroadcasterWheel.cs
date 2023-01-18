using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.Models
{
    public class BroadcasterWheel
    {
        public BroadcasterWheel() 
        {
        }

        public readonly int TimeBetweenBroadcastsInMilliseconds = 480000;

        public bool IsBroadcasting = false;

        public int SelectedMessage = 0;

        public readonly string[] Broadcast = { "", "", "", "", "" };
    }
}
