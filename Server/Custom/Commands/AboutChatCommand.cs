using LmpCommon.Message.Interface;
using Server.Client;
using Server.Custom.Handler;
using Server.Custom.Models;
using Server.Log;
using System;

namespace Server.Custom.Commands
{
    public class AboutChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static ChatCommands _chatCommands;

        public AboutChatCommand(MessageDispatcherHandler messageDispatcherHandler, ChatCommands chatCommands) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _chatCommands = chatCommands;
            LunaLog.Info($"AboutChatCommand object spawned");
        }

        public void AboutCommandHandler(string[] command, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"About Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient(_chatCommands.About, client);
        }
    }
}
