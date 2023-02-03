using JPCC.BaseStore;
using JPCC.Settings.Structures;
using Server.Settings.Structures;

namespace JPCC.Models
{
    public class ChatCommands
    {
        private static BaseKeeper _baseKeeper;

        private static Dictionary<string, string> enabledCommands;

        public ChatCommands(BaseKeeper baseKeeper)
        {
            _baseKeeper = baseKeeper;

            // Fetch needed variables
            About = _baseKeeper.About;
            WebsiteText = BaseSettings.SettingsStore.WebsiteUrl;

            // Check if the server administrator entered a valid command string in the config
            if (BaseSettings.SettingsStore.WebsiteCommand == "" || BaseSettings.SettingsStore.WebsiteCommand[0] != '/' || BaseSettings.SettingsStore.WebsiteCommand.Contains(' '))
            {
                throw new Exception("Invalid website command!");
            }

            // Dictionary for storing the enabled commands
            enabledCommands = new Dictionary<string, string>();

            // Check the commands array against the config to see what commands we should enable and add to the dictionary
            for (int i = 0; i < CommandsList.Count(); i++)
            {
                if (BaseSettings.SettingsStore.EnabledCommands.Contains(CommandsList[i]))
                {
                    enabledCommands.Add(CommandsList[i], CommandsDescriptionList[i]);
                }
            }

            // If no website text was set, remove the website command from the dictionary
            if (BaseSettings.SettingsStore.WebsiteUrl == "")
            {
                enabledCommands.Remove(BaseSettings.SettingsStore.WebsiteCommand);
            }
        }

        // Returns the dictionary object reference
        public Dictionary<string, string> GetEnabledCommands() 
        {
            return enabledCommands;    
        }

        public string About = "";

        public string WebsiteText = "";

        // *** The chat commands and their descriptions ***

        private readonly string[] CommandsList =
        {
            "/help",
            "/about",
            $"{BaseSettings.SettingsStore.WebsiteCommand}",
            "/msg",
            "/say",
            "/countdown",
            "/vote_resetworld",
            "/vote_kickplayer",
            "/vote_banplayer",
            "/yes",
            "/no"
        };

        private readonly string[] CommandsDescriptionList =
        {
            "/help <page number> - lists all commands",
            "/about - about J.P. Custom Commands",
            $"{BaseSettings.SettingsStore.WebsiteCommand} - returns the URL",
            "/msg <playername> <message text> - sends a private message to a player",
            "/say <message text> - say something as the server, people will still see you sent the message",
            "/countdown <5-30> - starts a countdown using the specified amount of seconds, useful for starting races",
            "/vote_resetworld - starts a vote on resetting the world",
            "/vote_kickplayer <playername> - starts a vote on kicking a player",
            "/vote_banplayer <playername> - starts a vote on banning a player",
            "/yes - for voting yes",
            "/no - for voting no"
        };
    }
}
