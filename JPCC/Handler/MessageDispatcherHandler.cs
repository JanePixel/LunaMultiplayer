using LmpCommon.Message;
using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Data.Motd;
using LmpCommon.Message.Server;
using Server.Client;
using Server.Context;
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

        // Method to send a chat message to a specific client
        public void DispatchMessageToSingleClient(string message, ClientStructure client) 
        {
            var messageData = ServerContext.ServerMessageFactory.CreateNewMessageData<ChatMsgData>();
            messageData.From = serverName;
            messageData.Relay = true;
            messageData.Text = message;

            MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
        }

        // Method to send a chat message to all clients
        public void DispatchMessageToAllClients(string message)
        {
            var messageData = ServerContext.ServerMessageFactory.CreateNewMessageData<ChatMsgData>();
            messageData.From = serverName;
            messageData.Relay = true;
            messageData.Text = message;

            MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);
        }

        // Method to send a MOTD message to a specific client
        public void DispatchMotd(string motd, ClientStructure client) 
        {
            var motdMessage = ServerContext.ServerMessageFactory.CreateNewMessageData<MotdReplyMsgData>();
            motdMessage.MessageOfTheDay = motd;

            MessageQueuer.SendToClient<MotdSrvMsg>(client, motdMessage);
        }
    }
}
