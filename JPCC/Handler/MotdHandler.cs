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
        private static string discordInfo = "\nWe have a Discord! Type /discord to join.";

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

            string motdToSend = defaultMotd
                .Replace("%Name%", client.PlayerName)
                .Replace("%ServerName%", GeneralSettings.SettingsStore.ServerName)
                .Replace("%PlayerCount%", ServerContext.Clients.Count.ToString())
                .Replace(@"\n", Environment.NewLine)
                + jpcc;

            if (BaseSettings.SettingsStore.EnabledCommands.Contains("/help")) 
            {
                motdToSend = motdToSend + helpCommandInfo;
            }
            if (BaseSettings.SettingsStore.EnabledCommands.Contains("/discord") && BaseSettings.SettingsStore.DiscordUrl != "") 
            {
                motdToSend = motdToSend + discordInfo;
            }

            _messageDispatcher.DispatchMotd(motdToSend, client);
        }
    }
}
