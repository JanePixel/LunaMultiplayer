using LmpCommon.Message.Data.Chat;
using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Commands;
using JPCC.Commands.SubHandler;
using JPCC.Models;
using Server.Log;

namespace JPCC.Handler
{
    public class ChatCommandsHandler
    {
        private static MessageDispatcherHandler _messageDispatcherHandler = new MessageDispatcherHandler();
        private static VotingTracker _votingTracker = new VotingTracker();
        private static CountdownTracker _countdownTracker = new CountdownTracker();
        private static readonly ChatCommands _chatCommands = new ChatCommands();

        private static RunVoteSubHandler _runVoteSubHandler = new RunVoteSubHandler(_messageDispatcherHandler, _votingTracker);
        private static RunCountdownSubHandler _runCountdownSubHandler = new RunCountdownSubHandler(_messageDispatcherHandler, _countdownTracker);

        private static InvalidChatCommand _invalidChatCommand = new InvalidChatCommand(_messageDispatcherHandler);
        private static HelpChatCommand _helpChatCommand = new HelpChatCommand(_messageDispatcherHandler, _chatCommands);
        private static AboutChatCommand _aboutChatCommand = new AboutChatCommand(_messageDispatcherHandler, _chatCommands);
        private static DiscordChatCommand _discordChatCommand = new DiscordChatCommand(_messageDispatcherHandler, _chatCommands);
        private static MsgChatCommand _msgChatCommand = new MsgChatCommand(_messageDispatcherHandler);
        private static SayChatCommand _sayChatCommand = new SayChatCommand(_messageDispatcherHandler);
        private static YesChatCommand _yesChatCommand = new YesChatCommand(_messageDispatcherHandler, _votingTracker);
        private static NoChatCommand _noChatCommand = new NoChatCommand(_messageDispatcherHandler, _votingTracker);
        private static VoteResetWorldChatCommand _voteResetWorldChatCommand = new VoteResetWorldChatCommand(_messageDispatcherHandler, _votingTracker, _runVoteSubHandler);
        private static VoteKickPlayerChatCommand _voteKickPlayerChatCommand = new VoteKickPlayerChatCommand(_messageDispatcherHandler, _votingTracker, _runVoteSubHandler);
        private static VoteBanPlayerChatCommand _voteBanPlayerChatCommand = new VoteBanPlayerChatCommand(_messageDispatcherHandler, _votingTracker, _runVoteSubHandler);
        private static CountdownChatCommand _countdownChatCommand = new CountdownChatCommand(_messageDispatcherHandler, _countdownTracker, _runCountdownSubHandler);

        public ChatCommandsHandler()
        {
            LunaLog.Info($"ChatCommandsHandler object spawned");
        }

        public void HandleChatCommand(ClientStructure client, IClientMessageBase message, ChatMsgData messageData)
        {
            // Inform server console about command usage and log it
            LunaLog.Info($"Player {messageData.From} used command: {messageData.Text}");

            var parsedCommand = messageData.Text.Split(' ');

            // Check what command we have
            switch (parsedCommand[0])
            {
                case var value when value == _chatCommands.CommandsList[0]:
                    // Help command handler
                    _helpChatCommand.HelpCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[1]:
                    // About command handler
                    _aboutChatCommand.AboutCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[2]:
                    // Msg command handler
                    _msgChatCommand.MsgCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[3]:
                    // Yes command handler
                    _yesChatCommand.YesCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[4]:
                    // No command handler
                    _noChatCommand.NoCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[5]:
                    // Reset world command handler
                    _voteResetWorldChatCommand.VoteResetWorldCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[6]:
                    // Kick player command handler
                    _voteKickPlayerChatCommand.VoteKickPlayerCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[7]:
                    // Ban player command handler
                    _voteBanPlayerChatCommand.VoteBanPlayerCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[8]:
                    // Say command handler
                    _sayChatCommand.SayCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[9]:
                    // Discord command handler
                    _discordChatCommand.DiscordCommandHandler(parsedCommand, client, message);
                    break;
                case var value when value == _chatCommands.CommandsList[10]:
                    // Countdown command handler
                    _countdownChatCommand.CountdownCommandHandler(parsedCommand, client, message);
                    break;
                default:
                    // No valid command found
                    _invalidChatCommand.InvalidCommandHandler(parsedCommand, client, message);
                    break;
            }
        }
    }
}
