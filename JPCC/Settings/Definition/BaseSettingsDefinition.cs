using LmpCommon.Enums;
using LmpCommon.Xml;
using System;

namespace JPCC.Settings.Definition
{
    [Serializable]
    public class BaseSettingsDefinition
    {
        [XmlComment(Value = "Should the default MOTD be overwritten? If enabled, JPCC’s presence will be broadcast to every player that joins.")]
        public bool OverrideDefaultMotd { get; set; } = true;

        [XmlComment(Value = "Add a URL to your discord server. Leave empty to disable.")]
        public string DiscordUrl { get; set; } = "https://discord.gg/SKqYxWHFth";

        [XmlComment(Value = "Should we enable the command system?")]
        public bool EnableCommands { get; set; } = true;

        [XmlComment(Value = "The commands that are enabled.")]
        public string EnabledCommands { get; set; } =
            "/help,\n" +
            "/about,\n" +
            "/discord,\n" +
            "/msg,\n" +
            "/say,\n" +
            "/countdown,\n" +
            "/vote_resetworld,\n" +
            "/vote_kickplayer,\n" +
            "/vote_banplayer,\n" +
            "/yes,\n" +
            "/no";
    }
}
