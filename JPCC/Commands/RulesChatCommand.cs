using Server.Client;
using JPCC.Handler;
using Server.Log;
using JPCC.Settings.Structures;

namespace JPCC.Commands
{
    // Rules chat command
    public class RulesChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;

        public RulesChatCommand(MessageDispatcherHandler messageDispatcherHandler) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
        }

        public void RulesCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Rules Command Handler activated for player {client.PlayerName}");

            _messageDispatcherHandler.DispatchMessageToSingleClient("\n" + BaseSettings.SettingsStore.RulesText, client);
        }
    }
}
