using JPCC.Settings.Base;
using JPCC.Settings.Definition;

namespace JPCC.Settings.Structures
{
    public class BackupAndRestoreSettings : SettingsBase<BackupAndRestoreSettingsDefinition>
    {
        protected override string Filename => "BackupAndRestoreSettings.xml";
    }
}
