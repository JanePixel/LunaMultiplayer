using Server.Client;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;

namespace JPCC.Commands
{
    // Help chat command
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

            // Get the total help page count by using the total number of entries vs allowed items per page
            totalPages = (int)Math.Ceiling(Convert.ToDouble(_chatCommands.GetEnabledCommands().Count) / Convert.ToDouble(itemsPerPage));
        }

        public void HelpCommandHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"Help Command Handler activated for player {client.PlayerName}");

            string helpOutput = "";
            int selectedPage = 1;

            try 
            {
                // If the user input enough parameters, check if the requested page is an integer
                if (command.Length > 1)
                {
                    selectedPage = int.Parse(command[1]);
                }

                // Is the input in the page range?
                if (selectedPage >= 1 && selectedPage <= totalPages)
                {
                    string commandsInFocus = "";

                    // Add the entries that should be on the selected page
                    for (int i = (((selectedPage - 1) * itemsPerPage) + 1); i <= (((selectedPage - 1) * itemsPerPage) + itemsPerPage); i++) 
                    {
                        if (i <= _chatCommands.GetEnabledCommands().Count())
                        {
                            commandsInFocus = commandsInFocus + _chatCommands.GetEnabledCommands().ElementAt(i-1).Value + "\n";
                        }
                    }

                    // Create the final message text the player will see
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
