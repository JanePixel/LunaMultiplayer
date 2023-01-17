using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.Models
{
    public class CountdownTracker
    {
        public CountdownTracker() 
        {
        
        }

        public bool IsCountdownRunning = false;

        public bool CanStartNewCountdown = true;

        public int SecondsCount = 0;
    }
}
