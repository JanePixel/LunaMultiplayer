using LmpCommon.Message.Interface;
using Server.Client;
using Server.Command;
using JPCC.Handler;
using JPCC.Models;
using Server.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.Commands.SubHandler
{
    public class RunCountdownSubHandler
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static CountdownTracker _countdownTracker;

        public RunCountdownSubHandler(MessageDispatcherHandler messageDispatcherHandler, CountdownTracker countdownTracker)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
            _countdownTracker = countdownTracker;
        }

        public void StartCountdownHandler(string[] command, ClientStructure client)
        {
            LunaLog.Info($"Start Countdown Sub Handler activated for player {client.PlayerName}");

            // Check if we already have a countdown running, if not proceed
            if (!_countdownTracker.IsCountdownRunning && _countdownTracker.CanStartNewCountdown)
            {
                _countdownTracker.IsCountdownRunning = true;
                _countdownTracker.CanStartNewCountdown = false;

                CountdownTimerAsync(command, client);
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("A countdown is currently running, can not start a new one!", client);
            }
        }

        private async Task CountdownTimerAsync(string[] command, ClientStructure client)
        {
            await Task.Delay(0100);

            _messageDispatcherHandler.DispatchMessageToAllClients($"Player {client.PlayerName} has started a countdown of {_countdownTracker.SecondsCount} seconds. Get ready!");
            LunaLog.Info($"Player {client.PlayerName} has started a countdown of {_countdownTracker.SecondsCount} seconds!");

            await Task.Delay(5000);

            // Count backwards every second
            while (_countdownTracker.SecondsCount >= 0)
            {
                if (((_countdownTracker.SecondsCount % 5) == 0) && (_countdownTracker.SecondsCount > 5)) // Is the current value a multiple of 5?
                {
                    AnnounceSecondsAsync(_countdownTracker.SecondsCount);
                }
                if ((_countdownTracker.SecondsCount <= 5) && (_countdownTracker.SecondsCount > 0)) // Count every second of the last five seconds
                {
                    AnnounceSecondsAsync(_countdownTracker.SecondsCount);
                }
                if (_countdownTracker.SecondsCount == 0) // We reached 0
                {
                    _messageDispatcherHandler.DispatchMessageToAllClients($"Go Go Go!");
                    LunaLog.Info($"Go! Countdown has finished!");
                }

                await Task.Delay(1000);
                _countdownTracker.SecondsCount = _countdownTracker.SecondsCount - 1;
            }

            // Reset state for next countdown
            _countdownTracker.SecondsCount = 0;
            _countdownTracker.IsCountdownRunning = false;
            _countdownTracker.CanStartNewCountdown = true;
        }

        private async Task AnnounceSecondsAsync(int seconds)
        {
            await Task.Delay(0010);
            _messageDispatcherHandler.DispatchMessageToAllClients($"T- {seconds} seconds");
            LunaLog.Info($"Countdown: T- {seconds} seconds");
        }
    }
}
