using LmpCommon.Xml;

namespace JPCC.Settings.Definition
{
    [Serializable]
    public class BaseSettingsDefinition
    {
        [XmlComment(Value = "Should the default MOTD be overwritten? If enabled, JPCC’s presence will be broadcast to every player that joins.")]
        public bool OverrideDefaultMotd { get; set; } = true;

        [XmlComment(Value = "Should the website be announced during join?")]
        public bool AnnounceWebsite { get; set; } = true;

        [XmlComment(Value = "The website text that will be added to the MOTD.")]
        public string WebsiteAnnounceText { get; set; } = "We have a Discord! Type /discord to join.";

        [XmlComment(Value = "The text that will be displayed when the website command is used.")]
        public string WebsiteUrl { get; set; } = "Link to public Discord server: https://discord.gg/SKqYxWHFth";

        [XmlComment(Value = "The custom command used for displaying the website URL. Make sure to update the EnabledCommands with the new command.")]
        public string WebsiteCommand { get; set; } = "/discord";

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
