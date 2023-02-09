using JPCC.BaseStore;
using JPCC.Settings.Structures;
using Server.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JPCC.Handler
{
    public class ResetWorldFilesHandler
    {
        private string saveFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "..\\..\\Universe\\";

        public ResetWorldFilesHandler() {}

        public void ResetWorld()
        {
            LunaLog.Info("Resetting the Universe Folder Files...");

            // Catch any errors while deleting the world folder files
            try 
            {
                string[] parsedResetItems = BackupAndRestoreSettings.SettingsStore.ItemsToReset.Split(",\n");

                // For each item in the array, check if it is a folder or a file, then delete it
                foreach (var item in parsedResetItems) 
                {
                    FileAttributes attr = File.GetAttributes(saveFilePath + item);

                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        Directory.Delete(saveFilePath + item, true);
                    }
                    else
                    {
                        File.Delete(saveFilePath + item);
                    }

                    LunaLog.Info($"Removed: {item}");
                }

                LunaLog.Info("All items removed successfully!");
            }
            catch (Exception ex) 
            {
                LunaLog.Error($"An error has occurred while resetting the Universe folder: {ex}");
            }
        }
    }
}
