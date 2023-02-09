using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Commands;
using JPCC.Commands.SubHandler;
using JPCC.Models;
using Server.Log;
using JPCC.BaseStore;
using JPCC.Settings.Structures;

namespace JPCC.Handler
{
    public class ChatCommandsHandler
    {
        private static BaseKeeper _baseKeeper;
        private static MessageDispatcherHandler _messageDispatcherHandler;
        
        private static VotingTracker _votingTracker;
        private static CountdownTracker _countdownTracker;
        private static ChatCommands _chatCommands;

        private static RunVoteSubHandler _runVoteSubHandler;
        private static RunCountdownSubHandler _runCountdownSubHandler;

        private static InvalidChatCommand _invalidChatCommand;
        private static HelpChatCommand _helpChatCommand;
        private static AboutChatCommand _aboutChatCommand;
        private static RulesChatCommand _rulesChatCommand;
        private static WebsiteChatCommand _websiteChatCommand;
        private static MsgChatCommand _msgChatCommand;
        private static SayChatCommand _sayChatCommand;
        private static YesChatCommand _yesChatCommand;
        private static NoChatCommand _noChatCommand;
        private static VoteResetWorldChatCommand _voteResetWorldChatCommand;
        private static VoteKickPlayerChatCommand _voteKickPlayerChatCommand;
        private static VoteBanPlayerChatCommand _voteBanPlayerChatCommand;
        private static CountdownChatCommand _countdownChatCommand;

        public ChatCommandsHandler(BaseKeeper baseKeeper, MessageDispatcherHandler messageDispatcherHandler)
        {
            _baseKeeper = baseKeeper;
            _messageDispatcherHandler = messageDispatcherHandler;

            _votingTracker = new VotingTracker();
            _countdownTracker = new CountdownTracker();
            _chatCommands = new ChatCommands(_baseKeeper);

            _runVoteSubHandler = new RunVoteSubHandler(_baseKeeper, _messageDispatcherHandler, _votingTracker);
            _runCountdownSubHandler = new RunCountdownSubHandler(_messageDispatcherHandler, _countdownTracker);

            _invalidChatCommand = new InvalidChatCommand(_messageDispatcherHandler);
            _helpChatCommand = new HelpChatCommand(_messageDispatcherHandler, _chatCommands);
            _aboutChatCommand = new AboutChatCommand(_messageDispatcherHandler, _chatCommands);
            _rulesChatCommand = new RulesChatCommand(_messageDispatcherHandler);
            _websiteChatCommand = new WebsiteChatCommand(_messageDispatcherHandler, _chatCommands);
            _msgChatCommand = new MsgChatCommand(_messageDispatcherHandler);
            _sayChatCommand = new SayChatCommand(_messageDispatcherHandler);
            _yesChatCommand = new YesChatCommand(_messageDispatcherHandler, _votingTracker);
            _noChatCommand = new NoChatCommand(_messageDispatcherHandler, _votingTracker);
            _voteResetWorldChatCommand = new VoteResetWorldChatCommand(_messageDispatcherHandler, _votingTracker, _runVoteSubHandler);
            _voteKickPlayerChatCommand = new VoteKickPlayerChatCommand(_messageDispatcherHandler, _votingTracker, _runVoteSubHandler);
            _voteBanPlayerChatCommand = new VoteBanPlayerChatCommand(_messageDispatcherHandler, _votingTracker, _runVoteSubHandler);
            _countdownChatCommand = new CountdownChatCommand(_messageDispatcherHandler, _countdownTracker, _runCountdownSubHandler);
        }

        public void CheckAndHandleChatCommand(ClientStructure client, IClientMessageBase message)
        {
            var chatMessage = (ChatMsgData)message.Data;
            var rawMessageText = chatMessage.Text;

            // Check for and intercept chat commands
            if (rawMessageText[0].ToString() == "/")
            {
                // Message was a command, use custom handler to deal with it
                message.Handled = true;

                HandleChatCommand(client, rawMessageText);
            }
        }

        private async Task HandleChatCommand(ClientStructure client, string rawMessageText)
        {
            await Task.Delay(0005);

            // Inform server console about command usage and log it
            LunaLog.Info($"Player {client.PlayerName} used command: {rawMessageText}");

            var parsedCommand = rawMessageText.Split(' ');

            // Get enabled commands
            var activeCommands = _chatCommands.GetEnabledCommands();

            // Check what command we have
            string commandBase = parsedCommand[0];
            
            bool foundCommand = false;

            if (commandBase == "/help" && activeCommands.ContainsKey("/help")) 
            {
                foundCommand = true;
                // Help command handler
                _helpChatCommand.HelpCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/about" && activeCommands.ContainsKey("/about"))
            {
                foundCommand = true;
                // About command handler
                _aboutChatCommand.AboutCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/rules" && activeCommands.ContainsKey("/rules"))
            {
                foundCommand = true;
                // Rules command handler
                _rulesChatCommand.RulesCommandHandler(parsedCommand, client);
            }
            if (commandBase == BaseSettings.SettingsStore.WebsiteCommand && activeCommands.ContainsKey(BaseSettings.SettingsStore.WebsiteCommand))
            {
                foundCommand = true;
                // Website command handler
                _websiteChatCommand.WebsiteCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/msg" && activeCommands.ContainsKey("/msg"))
            {
                foundCommand = true;
                // Msg command handler
                _msgChatCommand.MsgCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/say" && activeCommands.ContainsKey("/say"))
            {
                foundCommand = true;
                // Say command handler
                _sayChatCommand.SayCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/countdown" && activeCommands.ContainsKey("/countdown"))
            {
                foundCommand = true;
                // Countdown command handler
                _countdownChatCommand.CountdownCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/vote_resetworld" && activeCommands.ContainsKey("/vote_resetworld"))
            {
                foundCommand = true;
                // Reset world command handler
                _voteResetWorldChatCommand.VoteResetWorldCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/vote_kickplayer" && activeCommands.ContainsKey("/vote_kickplayer"))
            {
                foundCommand = true;
                // Kick player command handler
                _voteKickPlayerChatCommand.VoteKickPlayerCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/vote_banplayer" && activeCommands.ContainsKey("/vote_banplayer"))
            {
                foundCommand = true;
                // Ban player command handler
                _voteBanPlayerChatCommand.VoteBanPlayerCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/yes" && activeCommands.ContainsKey("/yes"))
            {
                foundCommand = true;
                // Yes command handler
                _yesChatCommand.YesCommandHandler(parsedCommand, client);
            }
            if (commandBase == "/no" && activeCommands.ContainsKey("/no"))
            {
                foundCommand = true;
                // No command handler
                _noChatCommand.NoCommandHandler(parsedCommand, client);
            }

            // No valid command found
            if (!foundCommand)
            {
                _invalidChatCommand.InvalidCommandHandler(parsedCommand, client); 
            }            
        }
    }
}
