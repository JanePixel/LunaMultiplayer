using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;

namespace JPCC.Commands
{
    // Custom website URL chat command
    public class WebsiteChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static ChatCommands _chatCommands;

        public WebsiteChatCommand(MessageDispatcherHandler messageDispatcherHandler, ChatCommands chatCommands) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _chatCommands = chatCommands;
        }

        public void WebsiteCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Website Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient(_chatCommands.WebsiteText, client);
        }
    }
}
