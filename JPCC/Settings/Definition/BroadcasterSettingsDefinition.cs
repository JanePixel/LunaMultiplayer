using LmpCommon.Enums;
using LmpCommon.Xml;
using System;

namespace JPCC.Settings.Definition
{
    [Serializable]
    public class BroadcasterSettingsDefinition
    {
        [XmlComment(Value = "Should we enable the broadcaster system?")]
        public bool EnableBroadcaster { get; set; } = true;

        [XmlComment(Value = "The interval in minutes between each message.")]
        public double BroadcasterIntervalInMinutes { get; set; } = 12;

        [XmlComment(Value = "The messages that will be broadcast.")]
        public string BroadcasterMessages { get; set; } =
            "\"This server uses chat commands! Use command /help to learn more.\",\n" +
            "\"Did you know that we have a public Discord server? Use command /discord to join!\",\n" +
            "\"Experiencing lag or low fps? A world reset may be in order! Use the command /vote_resetworld to start a vote on resetting the world.\",\n" +
            "\"Is someone annoying you? Take action! Use commands /vote_kickplayer and /vote_banplayer to start a vote on kicking or banning someone.\",\n" +
            "\"If you encounter any problems, feel free to report them in the problems and bugs channel of our public Discord server.\"";
    }
}
