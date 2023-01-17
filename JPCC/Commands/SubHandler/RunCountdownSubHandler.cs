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
            LunaLog.Info($"RunCountdownSubHandler object spawned");
        }

        public void StartCountdownHandler(string[] command, ClientStructure client, IClientMessageBase message)
        {
            LunaLog.Info($"Start Countdown Sub Handler activated for player {client.PlayerName}");

            if (!_countdownTracker.IsCountdownRunning && _countdownTracker.CanStartNewCountdown)
            {
                _countdownTracker.IsCountdownRunning = true;
                _countdownTracker.CanStartNewCountdown = false;

                CountdownTimerAsync(command, client, message);
            }
            else
            {
                _messageDispatcherHandler.DispatchMessageToSingleClient("A countdown is currently running, can not start a new one!", client);
            }
        }

        private async Task CountdownTimerAsync(string[] command, ClientStructure client, IClientMessageBase message)
        {
            await Task.Delay(0100);

            _messageDispatcherHandler.DispatchMessageToAllClients($"Player {client.PlayerName} has started a countdown of {_countdownTracker.SecondsCount} seconds. Get ready!");
            LunaLog.Info($"Player {client.PlayerName} has started a countdown of {_countdownTracker.SecondsCount} seconds!");

            await Task.Delay(5000);

            while (_countdownTracker.SecondsCount >= -1)
            {
                switch (_countdownTracker.SecondsCount)
                {
                    case 30:
                        AnnounceSecondsAsync(30);
                        break;
                    case 25:
                        AnnounceSecondsAsync(25);
                        break;
                    case 20:
                        AnnounceSecondsAsync(20);
                        break;
                    case 15:
                        AnnounceSecondsAsync(15);
                        break;
                    case 10:
                        AnnounceSecondsAsync(10);
                        break;
                    case 5:
                        AnnounceSecondsAsync(5);
                        break;
                    case 4:
                        AnnounceSecondsAsync(4);
                        break;
                    case 3:
                        AnnounceSecondsAsync(3);
                        break;
                    case 2:
                        AnnounceSecondsAsync(2);
                        break;
                    case 1:
                        AnnounceSecondsAsync(1);
                        break;
                    case 0:
                        _messageDispatcherHandler.DispatchMessageToAllClients($"Go Go Go!");
                        LunaLog.Info($"Go! Countdown has finished!");
                        break;
                }

                await Task.Delay(1000);
                _countdownTracker.SecondsCount = _countdownTracker.SecondsCount - 1;
            }

            _countdownTracker.SecondsCount = 0;
            _countdownTracker.IsCountdownRunning = false;
            _countdownTracker.CanStartNewCountdown = true;
        }

        private async Task AnnounceSecondsAsync(int seconds)
        {
            await Task.Delay(0010);
            _messageDispatcherHandler.DispatchMessageToAllClients($"T minus {seconds} seconds");
            LunaLog.Info($"Countdown: T minus {seconds} seconds");
        }
    }
}
