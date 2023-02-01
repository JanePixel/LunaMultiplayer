using JPCC.BaseStore;
using JPCC.Settings.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.Models
{
    public class ChatCommands
    {
        private static BaseKeeper _baseKeeper;

        public ChatCommands(BaseKeeper baseKeeper) 
        {
            _baseKeeper = baseKeeper;

            About = _baseKeeper.About;
            DiscordText = DiscordText + BaseSettings.SettingsStore.DiscordUrl;
        }

        public readonly string About = "";

        public readonly string DiscordText = "Link to public Discord server: ";

        public readonly string[] CommandsList = { "/help", "/about", "/msg", "/yes", "/no", "/vote_resetworld", "/vote_kickplayer", "/vote_banplayer", "/say", "/discord", "/countdown" };

        public readonly string[] CommandsDescriptionList = { "/help - lists all commands", "/about - about J.P. Custom Commands", "/msg <playername> <message text> - sends a private message to a player", "/yes - for voting yes", "/no - for voting no", "/vote_resetworld - starts a vote on resetting the world", "/vote_kickplayer <playername> - starts a vote on kicking a player", "/vote_banplayer <playername> - starts a vote on banning a player", "/say <message text> - say something as the server, people will still see you sent the message", "/discord - link to the Discord server", "/countdown <5-30> - starts a countdown using the specified amount of seconds, useful for starting races" };
    }
}
