using JPCC.Settings.Structures;
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
            TimeBetweenBroadcastsInMilliseconds = Convert.ToInt32(BroadcasterSettings.SettingsStore.BroadcasterIntervalInMinutes * 60000);
            Broadcast = BroadcasterSettings.SettingsStore.BroadcasterMessages.Trim('"').Split("\",\n\"");
        }

        public int TimeBetweenBroadcastsInMilliseconds { get; set; }

        public string[] Broadcast { get; set; }

        public bool IsBroadcasting = false;

        public int SelectedMessage = 0;
    }
}
