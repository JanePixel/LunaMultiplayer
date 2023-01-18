using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System;

namespace JPCC.Commands
{
    public class HelpChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static ChatCommands _chatCommands;

        public HelpChatCommand(MessageDispatcherHandler messageDispatcherHandler, ChatCommands chatCommands) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _chatCommands = chatCommands;
        }

        public void HelpCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Help Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient("Available commands are: " + Environment.NewLine + string.Join(Environment.NewLine, _chatCommands.CommandsDescriptionList), client);
        }
    }
}
