using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Custom.Models
{
    public class BroadcasterWheel
    {
        public BroadcasterWheel() 
        {
        }

        public readonly int TimeBetweenBroadcastsInMilliseconds = 300000;

        public int SelectedMessage = 0;

        public readonly string[] Broadcast = { "This server uses chat commands! Use command /help to learn more.", "Did you know that we have a public Discord server? Use command /discord to join!", "Experiencing lag or low fps? A world reset may be in order! Use the command /vote_resetworld to start a vote on resetting the world.", "Is someone annoying you? Take action! Use commands /vote_kickplayer and /vote_banplayer to start a vote on kicking or banning someone.", "If you encounter any problems, feel free to report them in the problems and bugs channel of our public Discord server." };
    }
}
