using Server.Client;
using JPCC.Commands.SubHandler;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;

namespace JPCC.Commands
{
    // Vote kick player chat command
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
        }

        public void VoteKickPlayerCommandHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"Vote Kick Player Command Handler activated for player {client.PlayerName}");

            // Do we have enough input parameters?
            if (command.Count() >= 2)
            {
                // Get target player
                var player = ClientRetriever.GetClientByName(command[1]);

                // Does the target player exist?
                if (player != null)
                {
                    // If no vote is already running proceed and start a new one
                    if (!_votingTracker.IsVoteRunning && _votingTracker.CanStartNewVote)
                    {
                        _votingTracker.VoteType = "kickplayer";
                    }

                    // Use vote subhandler to run vote
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
