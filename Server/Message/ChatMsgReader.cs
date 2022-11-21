using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Data.Motd;
using LmpCommon.Message.Interface;
using LmpCommon.Message.Server;
using Microsoft.VisualStudio.Threading;
using Server.Client;
using Server.Command;
using Server.Context;
using Server.Custom.Models;
using Server.Log;
using Server.Message.Base;
using Server.Server;
using Server.Settings.Structures;
using System;
using System.Linq;
using System.Threading.Tasks;
using uhttpsharp.Clients;

namespace Server.Message
{
    public class ChatMsgReader : ReaderBase
    {
        public static VotingTracker VoteTracker = new VotingTracker();

        public static readonly ChatCommands ChatCmds = new ChatCommands();

        public override void HandleMessage(ClientStructure client, IClientMessageBase message)
        {
            var messageData = (ChatMsgData)message.Data;

            if (messageData.Text[0].ToString() == "/")
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
            else
            {
                if (messageData.From != client.PlayerName) return;

                if (messageData.Relay)
                {
                    MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);
                    LunaLog.ChatMessage($"{messageData.From}: {messageData.Text}");
                }
                else //Is a PM to server msg
                {
                    LunaLog.Warning($"{messageData.From}: {messageData.Text}");
                }
            }
        }

        public void InvalidCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Invalid Command Handler activated for player {client.PlayerName}");

            var messageData = new ChatMsgData();
            messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            messageData.Relay = true;
            messageData.Text = "Beep Boop, command not recognized!";

            MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
        }

        public void HelpCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Help Command Handler activated for player {client.PlayerName}");

            var messageData = new ChatMsgData();
            messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            messageData.Relay = true;
            messageData.Text = "Available commands are: " + Environment.NewLine + string.Join(Environment.NewLine, chatCommands.CommandsDescriptionList);

            MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
        }

        public void AboutCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"About Command Handler activated for player {client.PlayerName}");

            var messageData = new ChatMsgData();
            messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            messageData.Relay = true;
            messageData.Text = "J.P. Custom Commands v0.0.1 by Jane Pixel.";

            MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
        }

        public void SayCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
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

                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = messageContentPrefix + messageContent;

                MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);
                LunaLog.Info($"{client.PlayerName} has sent everyone the following message: {messageContent}");
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Error, no message provided!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        public void MsgCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Private Message Command Handler activated for player {client.PlayerName}");

            if (!(command.Count() <= 2))
            {
                var player = ClientRetriever.GetClientByName(command[1]);

                if (player != null)
                {
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = $"Sending message to: {player.PlayerName}";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);

                    var messageDataToSend = new ChatMsgData();
                    messageDataToSend.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageDataToSend.Relay = true;
                    var messageContentPrefix = $"{client.PlayerName} has sent you a private message: ";
                    var messageContent = "";
                    for (var i = 2; i < command.Count(); i++)
                    {
                        messageContent = messageContent + command[i] + " ";
                    }
                    messageDataToSend.Text = messageContentPrefix + messageContent;

                    MessageQueuer.SendToClient<ChatSrvMsg>(player, messageDataToSend);

                    LunaLog.Info($"{client.PlayerName} sent {player.PlayerName} the following message: {messageContent}");
                }
                else
                {
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "Error, player not found!";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                }
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Error, no message or playername included!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        public void YesCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Yes Vote Command Handler activated for player {client.PlayerName}");

            if (VoteTracker.IsVoteRunning)
            {
                if (!(VoteTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    VoteTracker.PlayersWhoVoted.Add(client.PlayerName);
                    VoteTracker.VotedYesCount = VoteTracker.VotedYesCount + 1;

                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "You have voted! Please wait until the next vote in order to vote again.";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                    LunaLog.Info($"{client.PlayerName} has voted: yes");
                }
                else
                {
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "You can only vote once for a vote!";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                }
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Can not vote, no vote is running!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        public void NoCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"No Vote Command Handler activated for player {client.PlayerName}");

            if (VoteTracker.IsVoteRunning)
            {
                if (!(VoteTracker.PlayersWhoVoted.Contains(client.PlayerName)))
                {
                    VoteTracker.PlayersWhoVoted.Add(client.PlayerName);
                    VoteTracker.VotedNoCount = VoteTracker.VotedNoCount + 1;

                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "You have voted! Please wait until the next vote in order to vote again.";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                    LunaLog.Info($"{client.PlayerName} has voted: no");
                }
                else
                {
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "You can only vote once for a vote!";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                }
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Can not vote, no vote is running!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        public void VoteResetWorldCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Vote Reset World Command Handler activated for player {client.PlayerName}");

            StartVoteHandler(command, chatCommands, client, message, "resetworld");
        }

        public void VoteKickPlayerCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
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
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "Error, player not found!";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                }
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Error, playername not provided!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        public void VoteBanPlayerCommandHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message)
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
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "Error, player not found!";

                    MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
                }
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Error, playername not provided!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        public void StartVoteHandler(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
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
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = "A vote on resetting the world has been initiated! Please use the commands /yes or /no to cast your vote!";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);

                    LunaLog.Info($"{client.PlayerName} has started a vote on resetting the world!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
                if (VoteTracker.VoteType == "kickplayer")
                {
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = $"A vote on kicking {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);

                    LunaLog.Info($"{client.PlayerName} has started a vote on kicking {command[1]} from the server!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
                if (VoteTracker.VoteType == "banplayer")
                {
                    var messageData = new ChatMsgData();
                    messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    messageData.Relay = true;
                    messageData.Text = $"A vote on banning {command[1]} from the server has been initiated! Please use the commands /yes or /no to cast your vote!";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(messageData);

                    LunaLog.Info($"{client.PlayerName} has started a vote on banning {command[1]} from the server!");

                    VoteTimerAsync(command, chatCommands, client, message, voteType);
                }
            }
            else
            {
                var messageData = new ChatMsgData();
                messageData.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                messageData.Relay = true;
                messageData.Text = "Vote is currently running, can not start a new one!";

                MessageQueuer.SendToClient<ChatSrvMsg>(client, messageData);
            }
        }

        private async Task VoteTimerAsync(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            await Task.Delay(5000);

            var message1 = new ChatMsgData();
            message1.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            message1.Relay = true;
            message1.Text = "30 seconds left to vote!";

            MessageQueuer.SendToAllClients<ChatSrvMsg>(message1);
            LunaLog.Info($"Vote has 30 seconds left!");

            await Task.Delay(10000);

            var message2 = new ChatMsgData();
            message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            message2.Relay = true;
            message2.Text = "20 seconds left to vote!";

            MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
            LunaLog.Info($"Vote has 20 seconds left!");

            await Task.Delay(10000);

            var message3 = new ChatMsgData();
            message3.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            message3.Relay = true;
            message3.Text = "10 seconds left to vote!";

            MessageQueuer.SendToAllClients<ChatSrvMsg>(message3);
            LunaLog.Info($"Vote has 10 seconds left!");

            await Task.Delay(10000);
            VoteResultHandlerAsync(command, chatCommands, client, message, voteType);
        }

        private async Task VoteResultHandlerAsync(string[] command, ChatCommands chatCommands, ClientStructure client, IClientMessageBase message, string voteType)
        {
            await Task.Delay(0100);
            VoteTracker.IsVoteRunning = false;

            var message1 = new ChatMsgData();
            message1.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
            message1.Relay = true;
            message1.Text = $"Vote has finished! Results: {VoteTracker.PlayersWhoVoted.Count()} total votes, {VoteTracker.VotedYesCount.ToString()} voted yes, {VoteTracker.VotedNoCount.ToString()} voted no";

            MessageQueuer.SendToAllClients<ChatSrvMsg>(message1);
            LunaLog.Info($"Vote is over! Results: {VoteTracker.PlayersWhoVoted.Count()} total votes, {VoteTracker.VotedYesCount.ToString()} voted yes, {VoteTracker.VotedNoCount.ToString()} voted no");

            await Task.Delay(4000);

            if (voteType == "resetworld")
            {
                if (VoteTracker.VotedYesCount > VoteTracker.VotedNoCount)
                {
                    var message2 = new ChatMsgData();
                    message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message2.Relay = true;
                    message2.Text = $"Vote has succeeded! Enough players voted yes. World will be reset.";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. World will be reset.");

                    await Task.Delay(4000);

                    var message3 = new ChatMsgData();
                    message3.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message3.Relay = true;
                    message3.Text = $"Server will reboot in 5 seconds...";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message3);
                    LunaLog.Info($"Server will reboot in 5 seconds...");

                    await Task.Delay(5000);

                    MainServer.ResetWorldAndRestart();
                }
                else
                {
                    var message2 = new ChatMsgData();
                    message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message2.Relay = true;
                    message2.Text = $"Vote has failed! Not enough players voted yes. World will not be reset.";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. World will not be reset.");
                }
            }
            if (voteType == "kickplayer")
            {
                if ((VoteTracker.VotedYesCount > VoteTracker.VotedNoCount) && VoteTracker.PlayersWhoVoted.Count() >= 1)
                {
                    var message2 = new ChatMsgData();
                    message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message2.Relay = true;
                    message2.Text = $"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be kicked.");

                    await Task.Delay(2000);

                    var player = ClientRetriever.GetClientByName(command[1]);

                    if (player != null)
                    {
                        var kickMessage = "The server voted to kick you out!";
                        CommandHandler.Commands["kick"].Func($"{player.PlayerName} {kickMessage}");

                        var message3 = new ChatMsgData();
                        message3.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                        message3.Relay = true;
                        message3.Text = $"{command[1]} has been kicked!";

                        MessageQueuer.SendToAllClients<ChatSrvMsg>(message3);
                        LunaLog.Info($"{command[1]} has been kicked!");
                    }
                    else
                    {
                        var message3 = new ChatMsgData();
                        message3.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                        message3.Relay = true;
                        message3.Text = $"Error, {command[1]} could not be kicked as they are no longer on the server!";

                        MessageQueuer.SendToAllClients<ChatSrvMsg>(message3);
                        LunaLog.Info($"Error, {command[1]} could not be kicked as they are no longer on the server!");
                    }
                }
                else
                {
                    var message2 = new ChatMsgData();
                    message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message2.Relay = true;
                    message2.Text = $"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
                    LunaLog.Info($"Vote has failed! Not enough players voted yes. Player {command[1]} will not be kicked.");
                }
            }
            if (voteType == "banplayer")
            {
                if ((VoteTracker.VotedYesCount > VoteTracker.VotedNoCount) && VoteTracker.PlayersWhoVoted.Count() >= 2)
                {
                    var message2 = new ChatMsgData();
                    message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message2.Relay = true;
                    message2.Text = $"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
                    LunaLog.Info($"Vote has succeeded! Enough players voted yes. Player {command[1]} will be banned.");

                    await Task.Delay(2000);

                    var player = ClientRetriever.GetClientByName(command[1]);

                    if (player != null)
                    {
                        var banMessage = "The server voted to ban you!";
                        CommandHandler.Commands["ban"].Func($"{player.PlayerName} {banMessage}");

                        var message3 = new ChatMsgData();
                        message3.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                        message3.Relay = true;
                        message3.Text = $"{command[1]} has been banned!";

                        MessageQueuer.SendToAllClients<ChatSrvMsg>(message3);
                        LunaLog.Info($"{command[1]} has been banned!");
                    }
                    else
                    {
                        var message3 = new ChatMsgData();
                        message3.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                        message3.Relay = true;
                        message3.Text = $"Error, {command[1]} could not be banned as they are no longer on the server!";

                        MessageQueuer.SendToAllClients<ChatSrvMsg>(message3);
                        LunaLog.Info($"Error, {command[1]} could not be banned as they are no longer on the server!");
                    }
                }
                else
                {
                    var message2 = new ChatMsgData();
                    message2.From = GeneralSettings.SettingsStore.ConsoleIdentifier;
                    message2.Relay = true;
                    message2.Text = $"Vote has failed! Not enough players voted yes. Player {command[1]} will not be banned.";

                    MessageQueuer.SendToAllClients<ChatSrvMsg>(message2);
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
