using Server.Log;

namespace JPCC.Logging
{
    public class JPCCLog
    {
        private static readonly string JPCCString = "[JPCC]: ";

        static JPCCLog() {}

        #region Public methods

        public new static void NetworkVerboseDebug(string message)
        {
            LunaLog.NetworkVerboseDebug(JPCCString + message);
        }

        public new static void NetworkDebug(string message)
        {
            LunaLog.NetworkDebug(JPCCString + message);
        }

        public new static void Debug(string message)
        {
            LunaLog.Debug(JPCCString + message);
        }

        public new static void Warning(string message)
        {
            LunaLog.Warning(JPCCString + message);
        }

        public new static void Info(string message)
        {
            LunaLog.Info(JPCCString + message);
        }

        public new static void Normal(string message)
        {
            LunaLog.Normal(JPCCString + message);
        }

        public new static void Error(string message)
        {
            LunaLog.Error(JPCCString + message);
        }

        public new static void Fatal(string message)
        {
            LunaLog.Fatal(JPCCString + message);
        }

        public new static void ChatMessage(string message)
        {
            LunaLog.ChatMessage(JPCCString + message);
        }

        #endregion
    }
}
