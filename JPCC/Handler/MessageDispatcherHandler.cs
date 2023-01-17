using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Server;
using Server.Client;
using Server.Server;
using Server.Settings.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uhttpsharp.Clients;

namespace JPCC.Handler
{
    public class MessageDispatcherHandler
    {
        private static readonly string serverName = GeneralSettings.SettingsStore.ConsoleIdentifier;

        public MessageDispatcherHandler() { }

        public void DispatchMessageToSingleClient(string message, ClientStructure client) 
        {
            var messageData = new ChatMsgData();
            messageData.From = serverName;
            messageData.Relay = true;
            messageData.Text = message;

            MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
        }

        public void DispatchMessageToAllClients(string message)
        {
            var messageData = new ChatMsgData();
            messageData.From = serverName;
            messageData.Relay = true;
            messageData.Text = message;

            MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);
        }
    }
}
