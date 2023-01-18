using LmpCommon.Message.Interface;
using Server.Client;
using JPCC.Commands.SubHandler;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System;
using System.Linq;

namespace JPCC.Commands
{
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
            LunaLog.Info($"CountdownChatCommand object spawned");
        }

        public void CountdownCommandHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Countdown Command Handler activated for player {client.PlayerName}");

            if (command.Count() >= 2)
            {
                int seconds = 0;
                bool intParseState = false;

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

                if (intParseState) 
                {
                    if (seconds >= 5 && seconds <= 30) 
                    {
                        if (!_countdownTracker.IsCountdownRunning && _countdownTracker.CanStartNewCountdown) 
                        {
                            _countdownTracker.SecondsCount = seconds;
                        }
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
