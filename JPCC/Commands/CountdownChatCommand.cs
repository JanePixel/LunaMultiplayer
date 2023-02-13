using Server.Client;
using JPCC.Commands.SubHandler;
using JPCC.Handler;
using JPCC.Models;
using JPCC.Logging;

namespace JPCC.Commands
{
    // Countdown chat command
    public class CountdownChatCommand
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static CountdownTracker _countdownTracker;
        private static RunCountdownSubHandler _runCountdownSubHandler;

        public CountdownChatCommand(MessageDispatcherHandler messageDispatcherHandler, CountdownTracker countdownTracker, RunCountdownSubHandler runCountdownSubHandler)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _countdownTracker = countdownTracker;
            _runCountdownSubHandler = runCountdownSubHandler;
        }

        public void CountdownCommandHandler(string[] command, ClientStructure client)
        {
            JPCCLog.Debug($"Countdown Command Handler activated for player {client.PlayerName}");

            // Did the user input enough parameters?
            if (command.Count() >= 2)
            {
                int seconds = 0;
                bool intParseState = false;

                // Check if the user input a valid integer
                try
                {
                    seconds = Int32.Parse(command[1]);
                    intParseState = true;
                }
                catch (FormatException)
                {
                    intParseState = false;
                    _messageDispatcherHandler.DispatchMessageToSingleClient("Error, input must be an integer!", client);
                }

                // If the input was a valid integer, check if it is within the allowed range
                if (intParseState) 
                {
                    if (seconds >= 5 && seconds <= 30) 
                    {
                        // If no countdown is already running, set the count to the user input
                        if (!_countdownTracker.IsCountdownRunning && _countdownTracker.CanStartNewCountdown) 
                        {
                            _countdownTracker.SecondsCount = seconds;
                        }

                        // Use subhandler to run the countdown
                        _runCountdownSubHandler.StartCountdownHandler(command, client);
                    }
                    else 
                    {
                        _messageDispatcherHandler.DispatchMessageToSingleClient("Error, input must be in the range of 5-30!", client);
                    }
                }
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("Error, seconds not provided!", client);
            }
        }
    }
}
