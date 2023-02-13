using JPCC.Models;
using JPCC.Logging;

namespace JPCC.Handler
{
    public class BroadcastHandler
    {
        private static MessageDispatcherHandler _messageDispatcherHandler;
        private static BroadcasterWheel _broadcasterWheel = new BroadcasterWheel();

        public BroadcastHandler(MessageDispatcherHandler messageDispatcherHandler)
        {
            _messageDispatcherHandler = messageDispatcherHandler;
        }

        public void StartBroadcast()
        {
            // We don't want two or more broadcasters running
            if (!_broadcasterWheel.IsBroadcasting)
            {
                _broadcasterWheel.IsBroadcasting = true;
                BroadcasterLoop();
            }
        }

        private async Task BroadcasterLoop()
        {
            await Task.Delay(0100);
            JPCCLog.Normal($"Loaded {_broadcasterWheel.Broadcast.Count()} Broadcasts! Starting message Broadcasts with an interval of {_broadcasterWheel.TimeBetweenBroadcastsInMilliseconds / 60000} minutes!");

            _broadcasterWheel.SelectedMessage = 0;

            // Stay in this loop
            while (true)
            {
                // Wait the configured minute count between broadcasts
                await Task.Delay(_broadcasterWheel.TimeBetweenBroadcastsInMilliseconds);

                _messageDispatcherHandler.DispatchMessageToAllClients($"Broadcast: {_broadcasterWheel.Broadcast[_broadcasterWheel.SelectedMessage]}");
                JPCCLog.Debug($"Broadcast {_broadcasterWheel.SelectedMessage} dispatched!");

                // If we reached the end of the array, reset position back to 0, else add 1 to the position
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
