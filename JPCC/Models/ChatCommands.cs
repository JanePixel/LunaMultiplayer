using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.Models
{
    public class ChatCommands
    {
        public ChatCommands() 
        {
        
        }

        public readonly string About = "J.P. Custom Commands v1.0.2 by Jane Pixel. GitHub Repository: https://github.com/JanePixel/LunaMultiplayer";

        public readonly string DiscordText = "Link to public Discord server: https://discord.gg/SKqYxWHFth";

        public readonly string[] CommandsList = { "/help", "/about", "/msg", "/yes", "/no", "/vote_resetworld", "/vote_kickplayer", "/vote_banplayer", "/say", "/discord", "/countdown" };

        public readonly string[] CommandsDescriptionList = { "/help - lists all commands", "/about - about J.P. Custom Commands", "/msg <playername> <message text> - sends a private message to a player", "/yes - for voting yes", "/no - for voting no", "/vote_resetworld - starts a vote on resetting the world", "/vote_kickplayer <playername> - starts a vote on kicking a player", "/vote_banplayer <playername> - starts a vote on banning a player", "/say <message text> - say something as the server, people will still see you sent the message", "/discord - link to the Discord server", "/countdown <5-30> - starts a countdown using the specified amount of seconds, useful for starting races" };
    }
}
