using Server.Client;
using JPCC.Handler;
using JPCC.Logging;

namespace JPCC.Commands
{
    // Msg chat command
    public class MsgChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;

        public MsgChatCommand(MessageDispatcherHandler messageDispatcherHandler) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
        }

        public void MsgCommandHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"Private Message Command Handler activated for player {client.PlayerName}");

            ParseAndSendMessage(command, client);
        }

        private async Task ParseAndSendMessage(string[] command, ClientStructure client) 
        {
            await Task.Delay(0001);

            // Do we have enough input parameters?
            if (command.Count() >= 3)
            {
                // Get target player
                var player = ClientRetriever.GetClientByName(command[1]);

                // Check if target player exists
                if (player != null)
                {
                    var messageContentPrefix = $"{client.PlayerName} has sent you a private message:\n";
                    var messageContent = string.Join(" ", command.Skip(2));

                    JPCCLog.Normal($"{client.PlayerName} sent {player.PlayerName} the following message: {messageContent}");

                    _messageDispatcherHandler.DispatchMessageToSingleClient($"Sent {player.PlayerName} the following message:\n{messageContent}", client);

                    // Delay sending message due to lmp chat message duplication bug
                    await Task.Delay(1000);
                    _messageDispatcherHandler.DispatchMessageToSingleClient(messageContentPrefix + messageContent, player);
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToSingleClient("Error, player not found!", client);
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Error, no message or playername included!", client);
            }
        }
    }
}
