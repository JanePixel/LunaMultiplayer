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
                    case "flymode":
                        settingsKeeper.One = pair[1];
                        break;
                    case "mcmode":
                        settingsKeeper.Two = pair[1];
                        break;
                    default:
                        break;
                }
            }

            return settingsKeeper;
        }
    }
}
