using JPCC.Handler;
using JPCC.BaseStore;
using LmpCommon.Enums;
using LmpCommon.Message.Interface;
using Server;
using Server.Client;
using Server.Events;
using Server.Log;
using Server.Plugin;
using JPCC.Settings;
using JPCC.Settings.Structures;

namespace JPCC
{
    public class JPCCPlugin : ILmpPlugin
    {
        private static string version = "v2.0.0";
        public readonly string about = "J.P. Custom Commands " + version + " by Jane Pixel. GitHub Repository: https://github.com/JanePixel/LunaMultiplayer";

        private static bool loadingDone = false;

        private static BaseKeeper baseKeeper;

        private static MessageDispatcherHandler messageDispatcher;
        private static ChatCommandsHandler chatCommands;
        private static MotdHandler motdHandler;

        public virtual void OnUpdate()
        {
        }

        public virtual void OnServerStart()
        {
            //Try and load all classes and settings, in case of an error print output to console
            try
            {
                LunaLog.Info("Loading J.P.C.C. Systems and Settings...");

                baseKeeper = new BaseKeeper();
                baseKeeper.Version = version;
                baseKeeper.About = about;

                SettingsHandler.LoadSettings();

                messageDispatcher = new MessageDispatcherHandler();
                chatCommands = new ChatCommandsHandler(baseKeeper, messageDispatcher);
                motdHandler = new MotdHandler(messageDispatcher);

                loadingDone = true;
                LunaLog.Info("J.P.C.C. " + baseKeeper.Version + " Loaded!");
            }
            catch (Exception ex) 
            {
                LunaLog.Error($"Error! Could not load J.P.C.C.! Exception: {ex}");
            }
        }

        public virtual void OnServerStop()
        {
            //Reset the world if requested
            if (baseKeeper.ResetWorld) 
            {
                ResetWorldFilesHandler resetWorldFilesHandler = new ResetWorldFilesHandler();
                resetWorldFilesHandler.ResetWorld();
            }

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
            // Deal with chat commands if enabled
            if (loadingDone && messageData.MessageType == ClientMessageType.Chat && BaseSettings.SettingsStore.EnableCommands) 
            {
                chatCommands.CheckAndHandleChatCommand(client, messageData);
            }

            // Deal with the custom MOTD if enabled
            if (loadingDone && messageData.MessageType == ClientMessageType.Motd && BaseSettings.SettingsStore.OverrideDefaultMotd)
            {
                motdHandler.HandleMotd(client, messageData);
            }
        }

        public virtual void OnMessageSent(ClientStructure client, IServerMessageBase messageData)
        {
        }
    }
}