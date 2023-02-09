using JPCC.Settings.Base;
using System.Reflection;

namespace JPCC.Settings
{
    public static class SettingsHandler
    {
        public static void LoadSettings()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(ISettings).IsAssignableFrom(t) && !t.IsAbstract))
            {
                var instance = Activator.CreateInstance(type);
                ((ISettings)instance).Load();
            }
        }
    }
}
