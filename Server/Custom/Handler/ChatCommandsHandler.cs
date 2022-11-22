using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using Server.Client;
using Server.Command;
using Server.Custom.Commands;
using Server.Custom.Models;
using Server.Log;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Custom.Handler
{
    public class ChatCommandsHandler
    {
        private static MessageDispatcherHandler _messageDispatcherHandler = new MessageDispatcherHandler();
        private static VotingTracker _votingTracker = new VotingTracker();
        private static readonly ChatCommands _chatCommands = new ChatCommands();

        private static InvalidChatCommand _invalidChatCommand = new InvalidChatCommand(_messageDispatcherHandler);
        private static HelpChatCommand _helpChatCommand = new HelpChatCommand(_messageDispatcherHandler, _chatCommands);
        private static AboutChatCommand _aboutChatCommand = new AboutChatCommand(_messageDispatcherHandler, _chatCommands);
        private static MsgChatCommand _msgChatCommand = new MsgChatCommand(_messageDispatcherHandler);

        public ChatCommandsHandler() 
        {
            LunaLog.Info($"ChatCommandsHandler object spawned");
        }

        public void HandleChatCommand(ClientStructure client, IClientMessageBase message, ChatMsgData messageData) 
        {
            // Inform server console about command usage and log it
            LunaLog.Info($"Player {messageData.From} used command: {messageData.Text}");

            var parsedCommand = messageData.Text.Split(' ');

            var isValidCommand = false;

            // Help command parser
            if (parsedCommand[0] == _chatCommands.CommandsList[0])
            {
                isValidCommand = true;
                _helpChatCommand.HelpCommandHandler(parsedCommand, client, message);
            }

            // About command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[1])
            {
                isValidCommand = true;
                _aboutChatCommand.AboutCommandHandler(parsedCommand, client, message);
            }

            // Msg command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[2])
            {
                isValidCommand = true;
                _msgChatCommand.MsgCommandHandler(parsedCommand, client, message);
            }

            // Yes command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[3])
            {
                isValidCommand = true;
                YesCommandHandler(parsedCommand, client, message);
            }

            // No command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[4])
            {
                isValidCommand = true;
                NoCommandHandler(parsedCommand, client, message);
            }

            // Reset world command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[5])
            {
                isValidCommand = true;
                VoteResetWorldCommandHandler(parsedCommand, client, message);
            }

            // Kick player command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[6])
            {
                isValidCommand = true;
                VoteKickPlayerCommandHandler(parsedCommand, client, message);
            }

            // Ban player command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[7])
            {
                isValidCommand = true;
                VoteBanPlayerCommandHandler(parsedCommand, client, message);
            }

            // Say command handler
            if (parsedCommand[0] == _chatCommands.CommandsList[8])
            {
                isValidCommand = true;
                SayCommandHandler(parsedCommand, client, message);
            }

            // No valid command found
            if (!isValidCommand)
            {
                _invalidChatCommand.InvalidCommandHandler(parsedCommand, client, message);
            }
        }





        private void SayCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Say Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
                var messageContentPrefix = $"({client.PlayerName}): ";
                var messageContent = "";
                for (var i = 1; i < command.Count(); i++)
                {
                    messageContent = messageContent + command[i] + " ";
                }

                _messageDispatcherHandler.DispatchMessageToAllClients(messageContentPrefix + messageContent);
                LunaLog.Info($"{client.PlayerName} has sent everyone the following message: {messageContent}");
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Error, no message provided!", client);
            }
        }

       

        private void YesCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Yes Vote Command Handler activated for player {client.PlayerName}");

            if (_votingTracker.IsVoteRunning)
            {
                if (!(_votingTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    _votingTracker.PlayersWhoVoted.Add(client.PlayerName);
                    _votingTracker.VotedYesCount = _votingTracker.VotedYesCount + 1;

                    _messageDispatcherHandler.DispatchMessageToSingleClient("You have voted! Please wait until the next vote in order to vote again.", client);
                    LunaLog.Info($"{client.PlayerName} has voted: yes");
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

        private void NoCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
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

        private void VoteResetWorldCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Vote Reset World Command Handler activated for player {client.PlayerName}");

            StartVoteHandler(command, chatCommands, client, message, "resetworld");
        }

        private void VoteKickPlayerCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Vote Kick Player Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    StartVoteHandler(command, chatCommands, client, message, "kickplayer");
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

        private void VoteBanPlayerCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Vote Ban Player Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    StartVoteHandler(command, chatCommands, client, message, "banplayer");
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

        private void StartVoteHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            LunaLog.Info($"Start Vote Handler activated for player {client.PlayerName}");

            if (!_votingTracker.IsVoteRunning)
            {
                _votingTracker.IsVoteRunning = true;
                _votingTracker.VoteType = voteType;
                _votingTracker.PlayersWhoVoted.Clear();
                _votingTracker.VotedYesCount = 0;
                _votingTracker.VotedNoCount = 0;

                if (_votingTracker.VoteType == "resetworld")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients("A vote on resetting the world has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on resetting the world!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
                if (_votingTracker.VoteType == "kickplayer")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"A vote on kicking {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on kicking {command[1]} from the server!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
                if (_votingTracker.VoteType == "banplayer")
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"A vote on banning {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on banning {command[1]} from the server!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Vote is currently running, can not start a new one!", client);
            }
        }

        private async Task VoteTimerAsync(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
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
            VoteResultHandlerAsync(command, chatCommands, client, message, voteType);
        }

        private async Task VoteResultHandlerAsync(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            await Task.Delay(0100);
            _votingTracker.IsVoteRunning = false;

            _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has finished! Results: {_votingTracker.PlayersWhoVoted.Count()} total votes, {_votingTracker.VotedYesCount.ToString()} voted yes, {_votingTracker.VotedNoCount.ToString()} voted no");
            LunaLog.Info($"Vote is over! Results: {_votingTracker.PlayersWhoVoted.Count()} total votes, {_votingTracker.VotedYesCount.ToString()} voted yes, {_votingTracker.VotedNoCount.ToString()} voted no");

            await Task.Delay(4000);

            if (voteType == "resetworld")
            {
                if (_votingTracker.VotedYesCount > _votingTracker.VotedNoCount)
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. World will be reset.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. World will be reset.");

                    await Task.Delay(4000);

                    _messageDispatcherHandler.DispatchMessageToAllClients($"Server will reboot in 5 seconds...");
                    LunaLog.Info($"Server will reboot in 5 seconds...");

                    await Task.Delay(5000);

                    MainServer.ResetWorldAndRestart();
                }
                else
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. World will not be reset.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. World will not be reset.");
                }
            }
            if (voteType == "kickplayer")
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
            if (voteType == "banplayer")
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
        }
    }
}
