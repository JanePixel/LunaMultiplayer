using Server.Client;
using Server.Custom.Models;
using Server.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uhttpsharp.Clients;

namespace Server.Custom.Handler
{
    public class BroadcastHandler
    {
        private static MessageDispatcherHandler _messageDispatcherHandler = new MessageDispatcherHandler();
        private static BroadcasterWheel _broadcasterWheel = new BroadcasterWheel();

        public BroadcastHandler()
        {
            LunaLog.Info($"BroadcastHandler object spawned");
        }

        public void StartBroadcast()
        {
            if (!_broadcasterWheel.IsBroadcasting)
            {
                _broadcasterWheel.IsBroadcasting = true;
                BroadcasterLoop();
            }
        }

        private async Task BroadcasterLoop()
        {
            await Task.Delay(0100);
            LunaLog.Info($"A new Broadcaster instance has started!");

            _broadcasterWheel.SelectedMessage = 0;

            while (true)
            {
                await Task.Delay(_broadcasterWheel.TimeBetweenBroadcastsInMilliseconds);

                _messageDispatcherHandler.DispatchMessageToAllClients($"Broadcast: {_broadcasterWheel.Broadcast[_broadcasterWheel.SelectedMessage]}");
                LunaLog.Info($"Broadcast {_broadcasterWheel.SelectedMessage} dispatched!");

                if (_broadcasterWheel.SelectedMessage >= (_broadcasterWheel.Broadcast.Count() - 1))
                {
                    _broadcasterWheel.SelectedMessage = 0;
                }
                else
                {
                    _broadcasterWheel.SelectedMessage = _broadcasterWheel.SelectedMessage + 1;
                }
            }
        }
    }
}
