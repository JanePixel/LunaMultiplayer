using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System;

namespace JPCC.Commands
{
    public class DiscordChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static ChatCommands _chatCommands;

        public DiscordChatCommand(MessageDispatcherHandler messageDispatcherHandler, ChatCommands chatCommands) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _chatCommands = chatCommands;
            LunaLog.Info($"DiscordChatCommand object spawned");
        }

        public void DiscordCommandHandler(string[] command, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Discord Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient(_chatCommands.DiscordText, client);
        }
    }
}
