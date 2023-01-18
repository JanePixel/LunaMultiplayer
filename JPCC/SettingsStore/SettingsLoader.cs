using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.SettingsStore
{
    public class SettingsLoader
    {
        private string ConfigFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config";

        public SettingsLoader() { }

        public SettingsKeeper GetSettings() 
        {
            var settingsKeeper = new SettingsKeeper();

            var configFile = ConfigFolderPath + "\\Config.txt";

            var configLines = File.ReadAllLines(configFile);

            var configPairs = new List<string[]>();

            foreach (var line in configLines) 
            {
                var configPair = line.Split(" = ", 2);
                configPairs.Add(configPair);
            }

            foreach (var pair in configPairs) 
            {
                switch (pair[0])
                {
                    case "OverrideDefaultMotd":
                        settingsKeeper.OverrideDefaultMotd = bool.Parse(pair[1]);
                        break;
                    case "EnableCommands":
                        settingsKeeper.EnableCommands = bool.Parse(pair[1]);
                        break;
                    case "DiscordUrl":
                        settingsKeeper.DiscordUrl = pair[1];
                        break;
                    case "UniverseFoldersToReset":
                        settingsKeeper.UniverseFoldersToReset = ParseList(pair[1]);
                        break;
                    case "EnableBroadcaster":
                        settingsKeeper.EnableBroadcaster = bool.Parse(pair[1]);
                        break;
                    case "BroadcasterIntervalInMinutes":
                        settingsKeeper.BroadcasterIntervalInMinutes = double.Parse(pair[1]);
                        break;
                    case "BroadcasterMessages":
                        settingsKeeper.BroadcasterMessages = ParseList(pair[1]);
                        break;
                    default:
                        break;
                }
            }

            return settingsKeeper;
        }

        private List<string> ParseList(string rawList) 
        {
            List<string> parsedList = new List<string>();

            var items = rawList.Split("],[");

            foreach (var item in items) 
            {
                item.Replace("[", "");
                item.Replace("]", "");
                parsedList.Add(item);
            }

            return parsedList;
        }
    }
}
