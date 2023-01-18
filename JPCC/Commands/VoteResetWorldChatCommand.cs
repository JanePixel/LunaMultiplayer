using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Commands.SubHandler;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System.Linq;

namespace JPCC.Commands
{
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
            LunaLog.Info($"VoteResetWorldChatCommand object spawned");
        }

        public void VoteResetWorldCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Vote Reset World Command Handler activated for player {client.PlayerName}");

            if (!_votingTracker.IsVoteRunning && _votingTracker.CanStartNewVote) 
            {
                _votingTracker.VoteType = "resetworld";
            }

            _runVoteSubHandler.StartVoteHandler(command, client);
        }
    }
}
