using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;

namespace JPCC.Commands
{
    // User voted No chat command
    public class NoChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;

        public NoChatCommand(MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
        }

        public void NoCommandHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"No Vote Command Handler activated for player {client.PlayerName}");

            // Do we have a running vote? If yes, proceed
            if (_votingTracker.IsVoteRunning)
            {
                // Check if a player already voted
                if (!(_votingTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    // Player voted, add name to list and up the vote count
                    _votingTracker.PlayersWhoVoted.Add(client.PlayerName);
                    _votingTracker.VotedNoCount = _votingTracker.VotedNoCount + 1;

                    _messageDispatcherHandler.DispatchMessageToSingleClient("You have voted! Please wait until the next vote in order to vote again.", client);
                    JPCCLog.Normal($"{client.PlayerName} has voted: no");
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
