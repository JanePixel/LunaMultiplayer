using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Commands.SubHandler;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System.Linq;

namespace JPCC.Commands
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

        public void VoteKickPlayerCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Vote Kick Player Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    if (!_votingTracker.IsVoteRunning && _votingTracker.CanStartNewVote)
                    {
                        _votingTracker.VoteType = "kickplayer";
                    }

                    _runVoteSubHandler.StartVoteHandler(command, client);
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
