using Server.Client;
using Server.Command;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;
using JPCC.BaseStore;

namespace JPCC.Commands.SubHandler
{
    public class RunVoteSubHandler
    {
        private static BaseKeeper _baseKeeper;

        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;

        public RunVoteSubHandler(BaseKeeper baseKeeper, MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker) 
        {
            _baseKeeper = baseKeeper;

            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
        }

        public void StartVoteHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"Start Vote Sub Handler activated for player {client.PlayerName}");

            // Do we have a vote running already? If not, proceed
            if (!_votingTracker.IsVoteRunning && _votingTracker.CanStartNewVote)
            {
                // Set states
                _votingTracker.IsVoteRunning = true;
                _votingTracker.CanStartNewVote = false;
                _votingTracker.PlayersWhoVoted.Clear();
                _votingTracker.VotedYesCount = 0;
                _votingTracker.VotedNoCount = 0;

                // What type of vote do we have?
                if (_votingTracker.VoteType == "resetworld")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Player {client.PlayerName} has initiated a vote on resetting the world!{Environment.NewLine}Please use the commands /yes or /no to cast your vote!");
                    JPCCLog.Normal($"{client.PlayerName} has started a vote on resetting the world!");

                    VoteTimerAsync(command, client);
                }
                if (_votingTracker.VoteType == "kickplayer")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Player {client.PlayerName} has initiated a vote on kicking {command[1]} from the server!{Environment.NewLine}Please use the commands /yes or /no to cast your vote!");
                    JPCCLog.Normal($"{client.PlayerName} has started a vote on kicking {command[1]} from the server!");

                    VoteTimerAsync(command, client);
                }
                if (_votingTracker.VoteType == "banplayer")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Player {client.PlayerName} has initiated a vote on banning {command[1]} from the server!{Environment.NewLine}Please use the commands /yes or /no to cast your vote!");
                    JPCCLog.Normal($"{client.PlayerName} has started a vote on banning {command[1]} from the server!");

                    VoteTimerAsync(command, client);
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Vote is currently running, can not start a new one!", client);
            }
        }

        // Counter, used for all vote types
        private async Task VoteTimerAsync(string[] command, ClientStructure client)
        {
            await Task.Delay(5000);

            _messageDispatcherHandler.DispatchMessageToAllClients("30 seconds left to vote!");
            JPCCLog.Debug($"Vote has 30 seconds left!");

            await Task.Delay(10000);

            _messageDispatcherHandler.DispatchMessageToAllClients("20 seconds left to vote!");
            JPCCLog.Debug($"Vote has 20 seconds left!");

            await Task.Delay(10000);

            _messageDispatcherHandler.DispatchMessageToAllClients("10 seconds left to vote!");
            JPCCLog.Debug($"Vote has 10 seconds left!");

            await Task.Delay(10000);
            VoteResultHandlerAsync(command, client);
        }

        private async Task VoteResultHandlerAsync(string[] command, ClientStructure client)
        {
            await Task.Delay(0100);
            
            // Players will no longer be able to vote
            _votingTracker.IsVoteRunning = false;
            
            // Print vote reults
            _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has finished! Results:{Environment.NewLine}{_votingTracker.PlayersWhoVoted.Count()} total votes{Environment.NewLine}{_votingTracker.VotedYesCount.ToString()} voted yes{Environment.NewLine}{_votingTracker.VotedNoCount.ToString()} voted no");
            JPCCLog.Normal($"Vote is over! Results: {_votingTracker.PlayersWhoVoted.Count()} total votes, {_votingTracker.VotedYesCount.ToString()} voted yes, {_votingTracker.VotedNoCount.ToString()} voted no");
            
            // Use vote specific result handler methods
            await Task.Delay(4000);
            if (_votingTracker.VoteType == "resetworld")
            {
                await HandleResetVoteResults(command, client);
            }
            if (_votingTracker.VoteType == "kickplayer")
            {
                await HandleKickVoteResults(command, client);
            }
            if (_votingTracker.VoteType == "banplayer")
            {
                await HandleBanVoteResults(command, client);
            }

            // Reset the base state for the next vote
            _votingTracker.VoteType = "";
            _votingTracker.PlayersWhoVoted.Clear();
            _votingTracker.VotedYesCount = 0;
            _votingTracker.VotedNoCount = 0;
            _votingTracker.CanStartNewVote = true;
        }

        // Methods for dealing with the results

        // Handle the reset vote results
        private async Task HandleResetVoteResults(string[] command, ClientStructure client) 
        {
            await Task.Delay(0001);
            
            // Do we have enough votes?
            if (_votingTracker.VotedYesCount > _votingTracker.VotedNoCount)
            {
                _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. World will be reset.");
                JPCCLog.Normal($"Vote has succeeded! Enough players voted yes. World will be reset.");
                await Task.Delay(4000);

                _messageDispatcherHandler.DispatchMessageToAllClients($"Server will reboot in 5 seconds...");
                JPCCLog.Normal($"Server will reboot in 5 seconds...");

                await Task.Delay(5000);

                // Old reset logic, no longer used
                //MainServer.ResetWorldAndRestart();

                // Set reset state to true, then reboot
                _baseKeeper.ResetWorld = true;
                CommandHandler.Commands["restartserver"].Func(null);
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. World will not be reset.");
                JPCCLog.Normal($"Vote has failed! Not enough players voted yes. World will not be reset.");
            }
        }

        // Handle kick vote results
        private async Task HandleKickVoteResults(string[] command, ClientStructure client)
        {
            await Task.Delay(0001);

            // Do we have enough votes, and do we have more yes than no votes?
            if ((_votingTracker.VotedYesCount > _votingTracker.VotedNoCount) && _votingTracker.PlayersWhoVoted.Count() >= 1)
            {
                _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");
                JPCCLog.Normal($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");

                await Task.Delay(2000);

                // Fetch player details
                var player = ClientRetriever.GetClientByName(command[1]);

                // If player still online, proceed with kick
                if (player != null)
                {
                    var kickMessage = "The server voted to kick you out!";
                    CommandHandler.Commands["kick"].Func($"{player.PlayerName} {kickMessage}");

                    _messageDispatcherHandler.DispatchMessageToAllClients($"{command[1]} has been kicked!");
                    JPCCLog.Normal($"{command[1]} has been kicked!");
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                    JPCCLog.Normal($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
                JPCCLog.Normal($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
            }
        }

        // Handle the ban vote reults
        private async Task HandleBanVoteResults(string[] command, ClientStructure client)
        {
            await Task.Delay(0001);

            // Do we have enough votes and more yes than no votes?
            if ((_votingTracker.VotedYesCount > _votingTracker.VotedNoCount) && _votingTracker.PlayersWhoVoted.Count() >= 2)
            {
                _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");
                JPCCLog.Normal($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");

                await Task.Delay(2000);

                // Fetch player details
                var player = ClientRetriever.GetClientByName(command[1]);

                // If player still online, proceed with ban.
                // !!!!!!!!!!!!! Major loophole here as a player can quit before the vote runs out, thus not getting banned. Fix using database storing player ids.
                if (player != null)
                {
                    var banMessage = "The server voted to ban you!";
                    CommandHandler.Commands["ban"].Func($"{player.PlayerName} {banMessage}");

                    _messageDispatcherHandler.DispatchMessageToAllClients($"{command[1]} has been banned!");
                    JPCCLog.Normal($"{command[1]} has been banned!");
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Error, {command[1]} could not be banned as they are no longer on the server!");
                    JPCCLog.Normal($"Error, {command[1]} could not be banned as they are no longer on the server!");
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.");
                JPCCLog.Normal($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.");
            }
        }
    }
}
