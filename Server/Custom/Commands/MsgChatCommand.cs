using LmpCommon.Message.Interface;
using Server.Client;
using Server.Custom.Handler;
using Server.Custom.Models;
using Server.Log;
using System.Linq;

namespace Server.Custom.Commands
{
    public class MsgChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;

        public MsgChatCommand(MessageDispatcherHandler messageDispatcherHandler) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            LunaLog.Info($"MsgChatCommand object spawned");
        }

        public void MsgCommandHandler(string[] command, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Private Message Command Handler activated for player {client.PlayerName}");

            if (!(command.Count() <= 2))
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    _messageDispatcherHandler.DispatchMessageToSingleClient($"Sending message to: {player.PlayerName}", client);

                    var messageContentPrefix = $"{client.PlayerName} has sent you a private message: ";
                    var messageContent = "";
                    for (var i = 2; i < command.Count(); i++)
                    {
                        messageContent = messageContent + command[i] + " ";
                    }

                    _messageDispatcherHandler.DispatchMessageToSingleClient(messageContentPrefix + messageContent, player);

                    LunaLog.Info($"{client.PlayerName} sent {player.PlayerName} the following message: {messageContent}");
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
