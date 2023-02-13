using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;

namespace JPCC.Commands
{
    // User voted Yes chat command
    public class YesChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;

        public YesChatCommand(MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
        }

        public void YesCommandHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"Yes Vote Command Handler activated for player {client.PlayerName}");

            // Do we have a running vote?
            if (_votingTracker.IsVoteRunning)
            {
                // Check if player already voted on this vote
                if (!(_votingTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    // Add player to array and up the vote count
                    _votingTracker.PlayersWhoVoted.Add(client.PlayerName);
                    _votingTracker.VotedYesCount = _votingTracker.VotedYesCount + 1;

                    _messageDispatcherHandler.DispatchMessageToSingleClient("You have voted! Please wait until the next vote in order to vote again.", client);
                    JPCCLog.Normal($"{client.PlayerName} has voted: yes");
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToSingleClient("You can only vote once for a vote!", client);
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Can not vote, no vote is running!", client);
            }
        }
    }
}
