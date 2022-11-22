using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Data.Motd;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Server;
using Microsoft.VisualStudio.Threading;
using Server.Client;
using Server.Command;
using Server.Context;
using Server.Custom.Handler;
using Server.Custom.Models;
using Server.Log;
using Server.Message.Base;
using Server.Server;
using Server.Settings.Structures;
using System;
using System.Linq;
using System.Threading.Tasks;
using uhttpsharp.Clients;

namespace Server.Message
{
    public class ChatMsgReader : ReaderBase
    {
        public static ChatCommandsHandler CommandsHandler = new ChatCommandsHandler();

        public override void HandleMessage(ClientStructure client, IClientMessageBase message)
        {
            var messageData = (ChatMsgData)message.Data;

            // Check for and intercept chat commands
            if (messageData.Text[0].ToString() == "/")
            {
                // Message was a command, use handler to deal with it
                CommandsHandler.HandleChatCommand(client, message, messageData);
            }
            else
            {
                if (messageData.From != client.PlayerName) return;

                if (messageData.Relay)
                {
                    MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);
                    LunaLog.ChatMessage($"{messageData.From}: {messageData.Text}");
                }
                else //Is a PM to server msg
                {
                    LunaLog.Warning($"{messageData.From}: {messageData.Text}");
                }
            }
        }
    }
}
