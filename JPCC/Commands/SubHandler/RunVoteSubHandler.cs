using LmpCommon.Message.Interface;
using Server.Client;
using Server.Command;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPCC.SettingsStore;

namespace JPCC.Commands.SubHandler
{
    public class RunVoteSubHandler
    {
        private static SettingsKeeper _settingsKeeper;

        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static VotingTracker _votingTracker;

        public RunVoteSubHandler(SettingsKeeper settingsKeeper, MessageDispatcherHandler messageDispatcherHandler, VotingTracker votingTracker) 
        {
            _settingsKeeper = settingsKeeper;

            _messageDispatcherHandler = messageDispatcherHandler;
            _votingTracker = votingTracker;
        }

        public void StartVoteHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Start Vote Sub Handler activated for player {client.PlayerName}");

            if (!_votingTracker.IsVoteRunning && _votingTracker.CanStartNewVote)
            {
                _votingTracker.IsVoteRunning = true;
                _votingTracker.CanStartNewVote = false;
                _votingTracker.PlayersWhoVoted.Clear();
                _votingTracker.VotedYesCount = 0;
                _votingTracker.VotedNoCount = 0;

                if (_votingTracker.VoteType == "resetworld")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients("A vote on resetting the world has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on resetting the world!");

                    VoteTimerAsync(command, client);
                }
                if (_votingTracker.VoteType == "kickplayer")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"A vote on kicking {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on kicking {command[1]} from the server!");

                    VoteTimerAsync(command, client);
                }
                if (_votingTracker.VoteType == "banplayer")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"A vote on banning {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on banning {command[1]} from the server!");

                    VoteTimerAsync(command, client);
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Vote is currently running, can not start a new one!", client);
            }
        }

        private async Task VoteTimerAsync(string[] command, ClientStructure client)
        {
            await Task.Delay(5000);

            _messageDispatcherHandler.DispatchMessageToAllClients("30 seconds left to vote!");
            LunaLog.Info($"Vote has 30 seconds left!");

            await Task.Delay(10000);

            _messageDispatcherHandler.DispatchMessageToAllClients("20 seconds left to vote!");
            LunaLog.Info($"Vote has 20 seconds left!");

            await Task.Delay(10000);

            _messageDispatcherHandler.DispatchMessageToAllClients("10 seconds left to vote!");
            LunaLog.Info($"Vote has 10 seconds left!");

            await Task.Delay(10000);
            VoteResultHandlerAsync(command, client);
        }

        private async Task VoteResultHandlerAsync(string[] command, ClientStructure client)
        {
            await Task.Delay(0100);
            _votingTracker.IsVoteRunning = false;
            _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has finished! Results: {_votingTracker.PlayersWhoVoted.Count()} total votes, {_votingTracker.VotedYesCount.ToString()} voted yes, {_votingTracker.VotedNoCount.ToString()} voted no");
            LunaLog.Info($"Vote is over! Results: {_votingTracker.PlayersWhoVoted.Count()} total votes, {_votingTracker.VotedYesCount.ToString()} voted yes, {_votingTracker.VotedNoCount.ToString()} voted no");
            await Task.Delay(4000);
            if (_votingTracker.VoteType == "resetworld")
            {
                if (_votingTracker.VotedYesCount > _votingTracker.VotedNoCount)
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. World will be reset.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. World will be reset.");
                    await Task.Delay(4000);

                    _messageDispatcherHandler.DispatchMessageToAllClients($"Server will reboot in 5 seconds...");
                    LunaLog.Info($"Server will reboot in 5 seconds...");

                    await Task.Delay(5000);

                    //MainServer.ResetWorldAndRestart();

                    _settingsKeeper.ResetWorld = true;
                    CommandHandler.Commands["restartserver"].Func(null);
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. World will not be reset.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. World will not be reset.");
                }
            }
            if (_votingTracker.VoteType == "kickplayer")
            {
                if ((_votingTracker.VotedYesCount > _votingTracker.VotedNoCount) && _votingTracker.PlayersWhoVoted.Count() >= 1)
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");

                    await Task.Delay(2000);

                    var player = ClientRetriever.GetClientByName(command[1]);

                    if (player != null)
                    {
                        var kickMessage = "The server voted to kick you out!";
                        CommandHandler.Commands["kick"].Func($"{player.PlayerName} {kickMessage}");

                        _messageDispatcherHandler.DispatchMessageToAllClients($"{command[1]} has been kicked!");
                        LunaLog.Info($"{command[1]} has been kicked!");
                    }
                    else
                    {
                        _messageDispatcherHandler.DispatchMessageToAllClients($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                        LunaLog.Info($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                    }
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
                }
            }
            if (_votingTracker.VoteType == "banplayer")
            {
                if ((_votingTracker.VotedYesCount > _votingTracker.VotedNoCount) && _votingTracker.PlayersWhoVoted.Count() >= 2)
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");

                    await Task.Delay(2000);

                    var player = ClientRetriever.GetClientByName(command[1]);

                    if (player != null)
                    {
                        var banMessage = "The server voted to ban you!";
                        CommandHandler.Commands["ban"].Func($"{player.PlayerName} {banMessage}");

                        _messageDispatcherHandler.DispatchMessageToAllClients($"{command[1]} has been banned!");
                        LunaLog.Info($"{command[1]} has been banned!");
                    }
                    else
                    {
                        _messageDispatcherHandler.DispatchMessageToAllClients($"Error, {command[1]} could not be banned as they are no longer on the server!");
                        LunaLog.Info($"Error, {command[1]} could not be banned as they are no longer on the server!");
                    }
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.");
                }
            }

            _votingTracker.VoteType = "";
            _votingTracker.PlayersWhoVoted.Clear();
            _votingTracker.VotedYesCount = 0;
            _votingTracker.VotedNoCount = 0;
            _votingTracker.CanStartNewVote = true;
        }
    }
}
