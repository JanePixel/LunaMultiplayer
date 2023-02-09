namespace JPCC.Models
{
    // Keeps track of the state of a running countdown
    public class CountdownTracker
    {
        public CountdownTracker() {}

        public bool IsCountdownRunning = false;

        public bool CanStartNewCountdown = true;

        public int SecondsCount = 0;
    }
}
