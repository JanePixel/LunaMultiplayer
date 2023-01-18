using JPCC.Handler;
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
        public readonly string about = "J.P. Custom Commands " + version + " by Jane Pixel. GitHub Repository: https://github.com/JanePixel/LunaMultiplayer";

        private static bool loadingDone = false;

        private static SettingsLoader settingsLoader;
        private static SettingsKeeper settingsKeeper;

        private static ChatCommandsHandler chatCommands;

        public virtual void OnUpdate()
        {
        }

        public virtual void OnServerStart()
        {
            try
            {
                LunaLog.Info("Loading J.P.C.C. Systems and Settings...");
                
                chatCommands = new ChatCommandsHandler();
                settingsLoader = new SettingsLoader();
                
                settingsKeeper = settingsLoader.GetSettings();
                
                settingsKeeper.Version = version;
                settingsKeeper.About = about;
                
                loadingDone = true;
                LunaLog.Info("J.P.C.C. " + settingsKeeper.Version + " Loaded!");
            }
            catch (Exception ex) 
            {
                LunaLog.Error($"Error! Could not load J.P.C.C.! Exception: {ex}");
            }
        }

        public virtual void OnServerStop()
        {
            LunaLog.Info("J.P.C.C. is signing off!");
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
            if (loadingDone && settingsKeeper.EnableCommands && messageData.MessageType == ClientMessageType.Chat) 
            {
                chatCommands.CheckAndHandleChatCommand(client, messageData);
            }
        }

        public virtual void OnMessageSent(ClientStructure client, IServerMessageBase messageData)
        {
        }
    }
}