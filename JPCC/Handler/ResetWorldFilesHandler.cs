﻿using JPCC.Settings.Structures;
using JPCC.Logging;
using System.Reflection;

namespace JPCC.Handler
{
    public class ResetWorldFilesHandler
    {
        private string saveFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "..\\..\\Universe\\";

        public ResetWorldFilesHandler() {}

        public void ResetWorld()
        {
            JPCCLog.Normal("Resetting the Universe Folder Files...");

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

                    JPCCLog.Debug($"Removed: {item}");
                }

                JPCCLog.Normal("All items removed successfully!");
            }
            catch (Exception ex) 
            {
                JPCCLog.Error($"An error has occurred while resetting the Universe folder: {ex}");
            }
        }
    }
}
