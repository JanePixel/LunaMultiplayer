using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.SettingsStore
{
    public class SettingsKeeper
    {
        public SettingsKeeper() 
        {
        
        }

        public string Version = "";

        public string About = "";

        public bool OverrideDefaultMotd = false;

        public bool EnableCommands = false;

        public string DiscordUrl = "";

        public List<string> UniverseFoldersToReset;

        public bool EnableBroadcaster = false;

        public double BroadcasterIntervalInMinutes = 0.5;

        public List<string> BroadcasterMessages;
    }
}
