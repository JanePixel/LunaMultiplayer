using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;

namespace JPCC.Commands
{
    // About chat command
    public class AboutChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static ChatCommands _chatCommands;

        public AboutChatCommand(MessageDispatcherHandler messageDispatcherHandler, ChatCommands chatCommands) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _chatCommands = chatCommands;
        }

        public void AboutCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"About Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient(_chatCommands.About, client);
        }
    }
}
