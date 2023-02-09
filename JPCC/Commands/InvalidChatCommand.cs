using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Handler;
using Server.Log;

namespace JPCC.Commands
{
    // Handler for letting the user know that no valid command was used
    public class InvalidChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;

        public InvalidChatCommand(MessageDispatcherHandler messageDispatcherHandler) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
        }

        public void InvalidCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Invalid Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient("Beep Boop, command not recognized!", client);
        }
    }
}
