using LmpCommon.Message.Interface;
using Server.Client;
using Server.Custom.Commands.SubHandler;
using Server.Custom.Handler;
using Server.Custom.Models;
using Server.Log;
using System.Linq;

namespace Server.Custom.Commands
{
    public class VoteKickPlayerChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;
        private static RunVoteSubHandler _runVoteSubHandler;

        public VoteKickPlayerChatCommand(MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker, RunVoteSubHandler runVoteSubHandler)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
            _runVoteSubHandler = runVoteSubHandler;
            LunaLog.Info($"VoteKickPlayerChatCommand object spawned");
        }

        public void VoteKickPlayerCommandHandler(string[] command, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Vote Kick Player Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    _runVoteSubHandler.StartVoteHandler(command, client, message, "kickplayer");
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToSingleClient("Error, player not found!", client);
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Error, playername not provided!", client);
            }
        }
    }
}
