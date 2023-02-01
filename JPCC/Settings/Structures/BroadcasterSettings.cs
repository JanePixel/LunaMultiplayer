using JPCC.Settings.Base;
using JPCC.Settings.Definition;

namespace JPCC.Settings.Structures
{
    public class BroadcasterSettings : SettingsBase<BroadcasterSettingsDefinition>
    {
        protected override string Filename => "BroadcasterSettings.xml";
    }
}
