using LmpCommon.Message.Interface;
using Server.Client;
using Server.Custom.Handler;
using Server.Custom.Models;
using Server.Log;
using System.Linq;

namespace Server.Custom.Commands
{
    public class SayChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;

        public SayChatCommand(MessageDispatcherHandler messageDispatcherHandler)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            LunaLog.Info($"SayChatCommand object spawned");
        }

        public void SayCommandHandler(string[] command, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Say Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
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
