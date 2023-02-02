using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System;

namespace JPCC.Commands
{
    public class HelpChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static ChatCommands _chatCommands;

        private static int itemsPerPage = 4;
        private static int totalPages = 0;

        public HelpChatCommand(MessageDispatcherHandler messageDispatcherHandler, ChatCommands chatCommands) 
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _chatCommands = chatCommands;

            totalPages = (int)Math.Ceiling(Convert.ToDouble(_chatCommands.GetEnabledCommands().Count) / Convert.ToDouble(itemsPerPage));
        }

        public void HelpCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Help Command Handler activated for player {client.PlayerName}");

            string helpOutput = "";
            int selectedPage = 1;

            try 
            {
                if (command.Length > 1)
                {
                    selectedPage = int.Parse(command[1]);
                }

                if (selectedPage >= 1 && selectedPage <= totalPages)
                {
                    string commandsInFocus = "";

                    for (int i = (((selectedPage - 1) * itemsPerPage) + 1); i <= (((selectedPage - 1) * itemsPerPage) + itemsPerPage); i++) 
                    {
                        if (i <= _chatCommands.GetEnabledCommands().Count())
                        {
                            commandsInFocus = commandsInFocus + _chatCommands.GetEnabledCommands().ElementAt(i-1).Value + "\n";
                        }
                    }

                    helpOutput =
                        "\n<---Help Menu--->\n" +
                        commandsInFocus +
                        $"<---Page {selectedPage}/{totalPages}--->";

                    _messageDispatcherHandler.DispatchMessageToSingleClient(helpOutput, client);
                }
                else 
                {
                    _messageDispatcherHandler.DispatchMessageToSingleClient("Error, page number out of range!", client);
                }
            }
            catch (Exception ex) 
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Error, page must be a number!", client);
            }
        }
    }
}
