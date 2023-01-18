using JPCC.SettingsStore;
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
        private static SettingsKeeper _settingsKeeper;
        private string saveFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "..\\..\\Universe\\";

        public ResetWorldFilesHandler(SettingsKeeper settingsKeeper)
        {
            _settingsKeeper = settingsKeeper;
        }

        public void ResetWorld()
        {
            LunaLog.Info("Resetting the Universe Folder Files...");

            try 
            {
                foreach (var item in _settingsKeeper.UniverseFolderFilesToReset) 
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
