using JPCC.BaseStore;
using JPCC.Settings.Structures;

namespace JPCC.Models
{
    public class ChatCommands
    {
        private static BaseKeeper _baseKeeper;

        private static Dictionary<string, string> enabledCommands;

        public ChatCommands(BaseKeeper baseKeeper)
        {
            _baseKeeper = baseKeeper;

            About = _baseKeeper.About;
            DiscordText = DiscordText + BaseSettings.SettingsStore.DiscordUrl;

            enabledCommands = new Dictionary<string, string>();

            for (int i = 0; i < CommandsList.Count(); i++)
            {
                if (BaseSettings.SettingsStore.EnabledCommands.Contains(CommandsList[i]))
                {
                    enabledCommands.Add(CommandsList[i], CommandsDescriptionList[i]);
                }
            }

            if (BaseSettings.SettingsStore.DiscordUrl == "")
            {
                enabledCommands.Remove("/discord");
            }
        }

        public Dictionary<string, string> GetEnabledCommands() 
        {
            return enabledCommands;    
        }

        public string About = "";

        public string DiscordText = "Link to public Discord server: ";

        private readonly string[] CommandsList =
        {
            "/help",
            "/about",
            "/discord",
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
            "/discord - link to the Discord server",
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
