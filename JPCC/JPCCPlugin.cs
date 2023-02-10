using JPCC.Handler;
using JPCC.BaseStore;
using LmpCommon.Enums;
using LmpCommon.Message.Interface;
using Server.Client;
using Server.Log;
using Server.Plugin;
using JPCC.Settings;
using JPCC.Settings.Structures;

namespace JPCC
{
    public class JPCCPlugin : ILmpPlugin
    {
        // About and Version number
        private static readonly string version = "v2.0.0";
        public static readonly string about = "J.P. Custom Commands " + version + " by Jane Pixel. GitHub Repository: https://github.com/JanePixel/LunaMultiplayer";

        // Bool to signal loading completion
        private static bool loadingDone = false;

        private static BaseKeeper baseKeeper;

        private static MessageDispatcherHandler messageDispatcher;
        private static ChatCommandsHandler chatCommands;
        private static MotdHandler motdHandler;
        private static BroadcastHandler broadcastHandler;

        public virtual void OnServerStart()
        {
            //Try and load all classes and settings, in case of an error print output to console
            try
            {
                LunaLog.Info("Loading J.P.C.C. Systems and Settings...");

                // We use the BaseKeeper as a store for non settings related items
                baseKeeper = new BaseKeeper();
                baseKeeper.Version = version;
                baseKeeper.About = about;

                // Load settings
                SettingsHandler.LoadSettings();

                // Initialize objects
                messageDispatcher = new MessageDispatcherHandler();
                chatCommands = new ChatCommandsHandler(baseKeeper, messageDispatcher);
                motdHandler = new MotdHandler(messageDispatcher);
                broadcastHandler = new BroadcastHandler(messageDispatcher);

                //Everything loaded!
                loadingDone = true;
                LunaLog.Info("J.P.C.C. " + baseKeeper.Version + " Loaded!");

                //Start a broadcaster loop if enabled
                if (BroadcasterSettings.SettingsStore.EnableBroadcaster) 
                {
                    broadcastHandler.StartBroadcast();
                }
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

        public virtual void OnMessageReceived(ClientStructure client, IClientMessageBase messageData)
        {
            // Check what type of server message we have
            switch (messageData.MessageType)
            {
                case ClientMessageType.Chat:
                    HandleChatMessageType(client, messageData);
                    break;
                case ClientMessageType.Motd:
                    HandleMotdMessageType(client, messageData);
                    break;
            }
        }

        private void HandleChatMessageType(ClientStructure client, IClientMessageBase messageData)
        {
            // Deal with chat commands if enabled
            if (loadingDone && BaseSettings.SettingsStore.EnableCommands)
            {
                chatCommands.CheckAndHandleChatCommand(client, messageData);
            }
        }

        private void HandleMotdMessageType(ClientStructure client, IClientMessageBase messageData)
        {
            // Deal with the custom MOTD if enabled
            if (loadingDone && BaseSettings.SettingsStore.OverrideDefaultMotd)
            {
                motdHandler.HandleMotd(client, messageData);
            }
        }

        // ***************
        // Unused methods
        // ***************
        public virtual void OnUpdate() {}

        public virtual void OnMessageSent(ClientStructure client, IServerMessageBase messageData) {}

        public virtual void OnClientConnect(ClientStructure client) {}

        public virtual void OnClientAuthenticated(ClientStructure client) {}

        public virtual void OnClientDisconnect(ClientStructure client) {}
    }
}