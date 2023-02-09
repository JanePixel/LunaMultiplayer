using JPCC.Settings.Structures;
using LmpCommon.Message.Interface;
using Server.Client;
using Server.Context;
using Server.Settings.Structures;

namespace JPCC.Handler
{
    public class MotdHandler
    {
        private static string defaultMotd = GeneralSettings.SettingsStore.ServerMotd;
        private static string jpcc = "\nThis server is using J.P. Custom Commands!";
        private static string helpCommandInfo = "\nTo view all available commands, type /help in chat!";
        private static string rulesCommandInfo = "\nType /rules to see the server rules!";

        private static MessageDispatcherHandler _messageDispatcher;

        public MotdHandler(MessageDispatcherHandler messageDispatcherHandler) 
        {
            _messageDispatcher = messageDispatcherHandler;
        }

        public void HandleMotd(ClientStructure client, IClientMessageBase messageData) 
        {
            //We don't want the default MOTD to kick in
            messageData.Handled = true;

            SendMotd(client, messageData);
        }

        private async Task SendMotd(ClientStructure client, IClientMessageBase messageData) 
        {
            await Task.Delay(0001);

            // Convert the variables from the config
            string motdToSend = defaultMotd
                .Replace("%Name%", client.PlayerName)
                .Replace("%ServerName%", GeneralSettings.SettingsStore.ServerName)
                .Replace("%PlayerCount%", ServerContext.Clients.Count.ToString())
                .Replace(@"\n", Environment.NewLine)
                + jpcc;

            // If the help command is enabled, broadcast about it on join
            if (BaseSettings.SettingsStore.EnableCommands && BaseSettings.SettingsStore.EnabledCommands.Contains("/help")) 
            {
                motdToSend = motdToSend + helpCommandInfo;
            }
            // If the rules command is enabled and if broadcast rules info on join is enabled, broadcast about it on join
            if (BaseSettings.SettingsStore.EnableCommands && BaseSettings.SettingsStore.EnabledCommands.Contains("/rules") && BaseSettings.SettingsStore.RulesText != "" && BaseSettings.SettingsStore.AnnounceRules)
            {
                motdToSend = motdToSend + rulesCommandInfo;
            }
            // Are all conditions met to enable the broadcasting of the website command on join?
            if (BaseSettings.SettingsStore.EnableCommands && BaseSettings.SettingsStore.EnabledCommands.Contains(BaseSettings.SettingsStore.WebsiteCommand) && BaseSettings.SettingsStore.WebsiteAnnounceText != "" && BaseSettings.SettingsStore.AnnounceWebsite) 
            {
                motdToSend = motdToSend + "\n" + BaseSettings.SettingsStore.WebsiteAnnounceText;
            }

            _messageDispatcher.DispatchMotd(motdToSend, client);
        }
    }
}
