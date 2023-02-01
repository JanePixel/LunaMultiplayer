using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Commands;
using JPCC.Commands.SubHandler;
using JPCC.Models;
using Server.Log;
using LmpCommon.Message;
using JPCC.BaseStore;

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
        private static DiscordChatCommand _discordChatCommand;
        private static MsgChatCommand _msgChatCommand;
        private static SayChatCommand _sayChatCommand;
        private static YesChatCommand _yesChatCommand;
        private static NoChatCommand _noChatCommand;
        private static VoteResetWorldChatCommand _voteResetWorldChatCommand;
        private static VoteKickPlayerChatCommand _voteKickPlayerChatCommand;
        private static VoteBanPlayerChatCommand _voteBanPlayerChatCommand;
        private static CountdownChatCommand _countdownChatCommand;

        public ChatCommandsHandler(BaseKeeper baseKeeper)
        {
            _baseKeeper = baseKeeper;

            _messageDispatcherHandler = new MessageDispatcherHandler();
            _votingTracker = new VotingTracker();
            _countdownTracker = new CountdownTracker();
            _chatCommands = new ChatCommands(_baseKeeper);

            _runVoteSubHandler = new RunVoteSubHandler(_baseKeeper, _messageDispatcherHandler, _votingTracker);
            _runCountdownSubHandler = new RunCountdownSubHandler(_messageDispatcherHandler, _countdownTracker);

            _invalidChatCommand = new InvalidChatCommand(_messageDispatcherHandler);
            _helpChatCommand = new HelpChatCommand(_messageDispatcherHandler, _chatCommands);
            _aboutChatCommand = new AboutChatCommand(_messageDispatcherHandler, _chatCommands);
            _discordChatCommand = new DiscordChatCommand(_messageDispatcherHandler, _chatCommands);
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

            // Check what command we have
            switch (parsedCommand[0])
            {
                case var value when value == _chatCommands.CommandsList[0]:
                    // Help command handler
                    _helpChatCommand.HelpCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[1]:
                    // About command handler
                    _aboutChatCommand.AboutCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[2]:
                    // Msg command handler
                    _msgChatCommand.MsgCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[3]:
                    // Yes command handler
                    _yesChatCommand.YesCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[4]:
                    // No command handler
                    _noChatCommand.NoCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[5]:
                    // Reset world command handler
                    _voteResetWorldChatCommand.VoteResetWorldCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[6]:
                    // Kick player command handler
                    _voteKickPlayerChatCommand.VoteKickPlayerCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[7]:
                    // Ban player command handler
                    _voteBanPlayerChatCommand.VoteBanPlayerCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[8]:
                    // Say command handler
                    _sayChatCommand.SayCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[9]:
                    // Discord command handler
                    _discordChatCommand.DiscordCommandHandler(parsedCommand, client);
                    break;
                case var value when value == _chatCommands.CommandsList[10]:
                    // Countdown command handler
                    _countdownChatCommand.CountdownCommandHandler(parsedCommand, client);
                    break;
                default:
                    // No valid command found
                    _invalidChatCommand.InvalidCommandHandler(parsedCommand, client);
                    break;
            }
        }
    }
}
