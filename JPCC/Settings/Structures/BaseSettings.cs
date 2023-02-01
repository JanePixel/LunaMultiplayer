using JPCC.Settings.Base;
using JPCC.Settings.Definition;

namespace JPCC.Settings.Structures
{
    public class BaseSettings : SettingsBase<BaseSettingsDefinition>
    {
        protected override string Filename => "BaseSettings.xml";
    }
}
