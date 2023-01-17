using JPCC.SettingsStore;
using LmpCommon.Enums;
using LmpCommon.Message.Interface;
using Server;
using Server.Client;
using Server.Events;
using Server.Log;
using Server.Plugin;

namespace JPCC
{
    public class JPCCPlugin : ILmpPlugin
    {
        private static string version = "v2.0.0";

        private static SettingsLoader settingsLoader;
        private static SettingsKeeper settingsKeeper;

        public virtual void OnUpdate()
        {
        }

        public virtual void OnServerStart()
        {
            try
            {
                LunaLog.Info("Loading J.P.C.C. Settings...");
                settingsLoader = new SettingsLoader();
                settingsKeeper = settingsLoader.GetSettings();
                settingsKeeper.Version = version;
                LunaLog.Info("J.P.C.C. " + settingsKeeper.Version + " Loaded!");
            }
            catch (Exception ex) 
            {
                LunaLog.Error($"Error! Could not load J.P.C.C. Settings! Exception: {ex}");
            }
        }

        public virtual void OnServerStop()
        {
        }

        public virtual void OnClientConnect(ClientStructure client)
        {
        }

        public virtual void OnClientAuthenticated(ClientStructure client)
        {
        }

        public virtual void OnClientDisconnect(ClientStructure client)
        {
        }

        public virtual void OnMessageReceived(ClientStructure client, IClientMessageBase messageData)
        {
            if (messageData.MessageType == ClientMessageType.Chat) 
            {
                messageData.Handled = true;
            }
        }

        public virtual void OnMessageSent(ClientStructure client, IServerMessageBase messageData)
        {
        }
    }
}