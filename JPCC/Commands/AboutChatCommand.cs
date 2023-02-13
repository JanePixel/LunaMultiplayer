using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;

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
            JPCCLog.Debug($"About Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient(_chatCommands.About, client);
        }
    }
}
