using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System.Linq;

namespace JPCC.Commands
{
    public class NoChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;

        public NoChatCommand(MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
            LunaLog.Info($"NoChatCommand object spawned");
        }

        public void NoCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"No Vote Command Handler activated for player {client.PlayerName}");

            if (_votingTracker.IsVoteRunning)
            {
                if (!(_votingTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    _votingTracker.PlayersWhoVoted.Add(client.PlayerName);
                    _votingTracker.VotedNoCount = _votingTracker.VotedNoCount + 1;

                    _messageDispatcherHandler.DispatchMessageToSingleClient("You have voted! Please wait until the next vote in order to vote again.", client);
                    LunaLog.Info($"{client.PlayerName} has voted: no");
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
