using Server.Client;
using JPCC.Handler;
using Server.Log;

namespace JPCC.Commands
{
    // Say chat command
    public class SayChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;

        public SayChatCommand(MessageDispatcherHandler messageDispatcherHandler)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
        }

        public void SayCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Say Command Handler activated for player {client.PlayerName}");

            // Check if we have enough input parameters
            if (command.Count() >= 2)
            {
                // Construct the message
                var messageContentPrefix = $"({client.PlayerName}): ";
                var messageContent = "";
                for (var i = 1; i < command.Count(); i++)
                {
                    messageContent = messageContent + command[i] + " ";
                }

                _messageDispatcherHandler.DispatchMessageToAllClients(messageContentPrefix + messageContent);
                LunaLog.Info($"{client.PlayerName} has sent everyone the following message: {messageContent}");
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Error, no message provided!", client);
            }
        }
    }
}
