using Server.Client;
using JPCC.Commands.SubHandler;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;

namespace JPCC.Commands
{
    // Vote reset world chat command
    public class VoteResetWorldChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;
        private static RunVoteSubHandler _runVoteSubHandler;

        public VoteResetWorldChatCommand(MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker, RunVoteSubHandler runVoteSubHandler)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
            _runVoteSubHandler= runVoteSubHandler;
        }

        public void VoteResetWorldCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Vote Reset World Command Handler activated for player {client.PlayerName}");

            // Do we already have a vote running? If not, proceed and start a new one
            if (!_votingTracker.IsVoteRunning && _votingTracker.CanStartNewVote) 
            {
                _votingTracker.VoteType = "resetworld";
            }

            // Use vote subhandler to run vote
            _runVoteSubHandler.StartVoteHandler(command, client);
        }
    }
}
