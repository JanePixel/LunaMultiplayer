using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Server;
using Server.Client;
using Server.Command;
using Server.Custom.Models;
using Server.Log;
using Server.Server;
using Server.Settings.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Custom.Handler
{
    public class ChatCommandsHandler
    {
        private static MessageDispatcherHandler MessageDispatcher = new MessageDispatcherHandler();

        private static VotingTracker VoteTracker = new VotingTracker();

        private static readonly ChatCommands ChatCmds = new ChatCommands();

        public ChatCommandsHandler() { }

        public void HandleChatCommand(ClientStructure client, IClientMessageBase message, ChatMsgData messageData) 
        {
            // Inform server console about command usage and log it
            LunaLog.Info($"Player {messageData.From} used command: {messageData.Text}");

            var parsedCommand = messageData.Text.Split(' ');

            var isValidCommand = false;

            // Help command parser
            if (parsedCommand[0] == ChatCmds.CommandsList[0])
            {
                isValidCommand = true;
                HelpCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // About command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[1])
            {
                isValidCommand = true;
                AboutCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // Msg command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[2])
            {
                isValidCommand = true;
                MsgCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // Yes command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[3])
            {
                isValidCommand = true;
                YesCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // No command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[4])
            {
                isValidCommand = true;
                NoCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // Reset world command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[5])
            {
                isValidCommand = true;
                VoteResetWorldCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // Kick player command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[6])
            {
                isValidCommand = true;
                VoteKickPlayerCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // Ban player command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[7])
            {
                isValidCommand = true;
                VoteBanPlayerCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // Say command handler
            if (parsedCommand[0] == ChatCmds.CommandsList[8])
            {
                isValidCommand = true;
                SayCommandHandler(parsedCommand, ChatCmds, client, message);
            }

            // No valid command found
            if (!isValidCommand)
            {
                InvalidCommandHandler(parsedCommand, ChatCmds, client, message);
            }
        }

        private void InvalidCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Invalid Command Handler activated for player {client.PlayerName}");
            
            MessageDispatcher.DispatchMessageToSingleClient("Beep Boop, command not recognized!", client);
        }

        private void HelpCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Help Command Handler activated for player {client.PlayerName}");

            MessageDispatcher.DispatchMessageToSingleClient("Available commands are: " + Environment.NewLine + string.Join(Environment.NewLine, chatCommands.CommandsDescriptionList), client);
        }

        private void AboutCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"About Command Handler activated for player {client.PlayerName}");

            MessageDispatcher.DispatchMessageToSingleClient(ChatCmds.About, client);
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

                MessageDispatcher.DispatchMessageToAllClients(messageContentPrefix + messageContent);
                LunaLog.Info($"{client.PlayerName} has sent everyone the following message: {messageContent}");
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Error, no message provided!", client);
            }
        }

        private void MsgCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Private Message Command Handler activated for player {client.PlayerName}");

            if (!(command.Count() <= 2))
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    MessageDispatcher.DispatchMessageToSingleClient($"Sending message to: {player.PlayerName}", client);

                    var messageContentPrefix = $"{client.PlayerName} has sent you a private message: ";
                    var messageContent = "";
                    for (var i = 2; i < command.Count(); i++)
                    {
                        messageContent = messageContent + command[i] + " ";
                    }
                  
                    MessageDispatcher.DispatchMessageToSingleClient(messageContentPrefix + messageContent, player);

                    LunaLog.Info($"{client.PlayerName} sent {player.PlayerName} the following message: {messageContent}");
                }
                else
                {
                    MessageDispatcher.DispatchMessageToSingleClient("Error, player not found!", client);
                }
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Error, no message or playername included!", client);
            }
        }

        private void YesCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Yes Vote Command Handler activated for player {client.PlayerName}");

            if (VoteTracker.IsVoteRunning)
            {
                if (!(VoteTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    VoteTracker.PlayersWhoVoted.Add(client.PlayerName);
                    VoteTracker.VotedYesCount = VoteTracker.VotedYesCount + 1;

                    MessageDispatcher.DispatchMessageToSingleClient("You have voted! Please wait until the next vote in order to vote again.", client);
                    LunaLog.Info($"{client.PlayerName} has voted: yes");
                }
                else
                {
                    MessageDispatcher.DispatchMessageToSingleClient("You can only vote once for a vote!", client);
                }
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Can not vote, no vote is running!", client);
            }
        }

        private void NoCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"No Vote Command Handler activated for player {client.PlayerName}");

            if (VoteTracker.IsVoteRunning)
            {
                if (!(VoteTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    VoteTracker.PlayersWhoVoted.Add(client.PlayerName);
                    VoteTracker.VotedNoCount = VoteTracker.VotedNoCount + 1;

                    MessageDispatcher.DispatchMessageToSingleClient("You have voted! Please wait until the next vote in order to vote again.", client);
                    LunaLog.Info($"{client.PlayerName} has voted: no");
                }
                else
                {
                    MessageDispatcher.DispatchMessageToSingleClient("You can only vote once for a vote!", client);
                }
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Can not vote, no vote is running!", client);
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
                    MessageDispatcher.DispatchMessageToSingleClient("Error, player not found!", client);
                }
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Error, playername not provided!", client);
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
                    MessageDispatcher.DispatchMessageToSingleClient("Error, player not found!", client);
                }
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Error, playername not provided!", client);
            }
        }

        private void StartVoteHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            LunaLog.Info($"Start Vote Handler activated for player {client.PlayerName}");

            if (!VoteTracker.IsVoteRunning)
            {
                VoteTracker.IsVoteRunning = true;
                VoteTracker.VoteType = voteType;
                VoteTracker.PlayersWhoVoted.Clear();
                VoteTracker.VotedYesCount = 0;
                VoteTracker.VotedNoCount = 0;

                if (VoteTracker.VoteType == "resetworld")
                {
                    MessageDispatcher.DispatchMessageToAllClients("A vote on resetting the world has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on resetting the world!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
                if (VoteTracker.VoteType == "kickplayer")
                {
                    MessageDispatcher.DispatchMessageToAllClients($"A vote on kicking {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on kicking {command[1]} from the server!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
                if (VoteTracker.VoteType == "banplayer")
                {
                    MessageDispatcher.DispatchMessageToAllClients($"A vote on banning {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!");
                    LunaLog.Info($"{client.PlayerName} has started a vote on banning {command[1]} from the server!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
            }
            else
            {
                MessageDispatcher.DispatchMessageToSingleClient("Vote is currently running, can not start a new one!", client);
            }
        }

        private async Task VoteTimerAsync(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            await Task.Delay(5000);

            MessageDispatcher.DispatchMessageToAllClients("30 seconds left to vote!");
            LunaLog.Info($"Vote has 30 seconds left!");

            await Task.Delay(10000);

            MessageDispatcher.DispatchMessageToAllClients("20 seconds left to vote!");
            LunaLog.Info($"Vote has 20 seconds left!");

            await Task.Delay(10000);

            MessageDispatcher.DispatchMessageToAllClients("10 seconds left to vote!");
            LunaLog.Info($"Vote has 10 seconds left!");

            await Task.Delay(10000);
            VoteResultHandlerAsync(command, chatCommands, client, message, voteType);
        }

        private async Task VoteResultHandlerAsync(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            await Task.Delay(0100);
            VoteTracker.IsVoteRunning = false;

            MessageDispatcher.DispatchMessageToAllClients($"Vote has finished! Results: {VoteTracker.PlayersWhoVoted.Count()} total votes, {VoteTracker.VotedYesCount.ToString()} voted yes, {VoteTracker.VotedNoCount.ToString()} voted no");
            LunaLog.Info($"Vote is over! Results: {VoteTracker.PlayersWhoVoted.Count()} total votes, {VoteTracker.VotedYesCount.ToString()} voted yes, {VoteTracker.VotedNoCount.ToString()} voted no");

            await Task.Delay(4000);

            if (voteType == "resetworld")
            {
                if (VoteTracker.VotedYesCount > VoteTracker.VotedNoCount)
                {
                    MessageDispatcher.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. World will be reset.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. World will be reset.");

                    await Task.Delay(4000);

                    MessageDispatcher.DispatchMessageToAllClients($"Server will reboot in 5 seconds...");
                    LunaLog.Info($"Server will reboot in 5 seconds...");

                    await Task.Delay(5000);

                    MainServer.ResetWorldAndRestart();
                }
                else
                {
                    MessageDispatcher.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. World will not be reset.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. World will not be reset.");
                }
            }
            if (voteType == "kickplayer")
            {
                if ((VoteTracker.VotedYesCount > VoteTracker.VotedNoCount) && VoteTracker.PlayersWhoVoted.Count() >= 1)
                {
                    MessageDispatcher.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");

                    await Task.Delay(2000);

                    var player = ClientRetriever.GetClientByName(command[1]);

                    if (player != null)
                    {
                        var kickMessage = "The server voted to kick you out!";
                        CommandHandler.Commands["kick"].Func($"{player.PlayerName} {kickMessage}");

                        MessageDispatcher.DispatchMessageToAllClients($"{command[1]} has been kicked!");
                        LunaLog.Info($"{command[1]} has been kicked!");
                    }
                    else
                    {
                        MessageDispatcher.DispatchMessageToAllClients($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                        LunaLog.Info($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                    }
                }
                else
                {
                    MessageDispatcher.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
                }
            }
            if (voteType == "banplayer")
            {
                if ((VoteTracker.VotedYesCount > VoteTracker.VotedNoCount) && VoteTracker.PlayersWhoVoted.Count() >= 2)
                {
                    MessageDispatcher.DispatchMessageToAllClients($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");

                    await Task.Delay(2000);

                    var player = ClientRetriever.GetClientByName(command[1]);

                    if (player != null)
                    {
                        var banMessage = "The server voted to ban you!";
                        CommandHandler.Commands["ban"].Func($"{player.PlayerName} {banMessage}");

                        MessageDispatcher.DispatchMessageToAllClients($"{command[1]} has been banned!");
                        LunaLog.Info($"{command[1]} has been banned!");
                    }
                    else
                    {
                        MessageDispatcher.DispatchMessageToAllClients($"Error, {command[1]} could not be banned as they are no longer on the server!");
                        LunaLog.Info($"Error, {command[1]} could not be banned as they are no longer on the server!");
                    }
                }
                else
                {
                    MessageDispatcher.DispatchMessageToAllClients($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.");
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.");
                }
            }

            VoteTracker.VoteType = "";
            VoteTracker.PlayersWhoVoted.Clear();
            VoteTracker.VotedYesCount = 0;
            VoteTracker.VotedNoCount = 0;
        }
    }
}
