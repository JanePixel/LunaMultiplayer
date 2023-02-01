using LmpCommon.Enums;
using LmpCommon.Xml;
using System;

namespace JPCC.Settings.Definition
{
    [Serializable]
    public class BackupAndRestoreSettingsDefinition
    {
        [XmlComment(Value = "Items and folders to delete when a world reset is called upon.")]
        public string ItemsToReset { get; set; } =
            "Subspace.txt,\n" +
            "StartTime.txt,\n" +
            "Vessels,\n" +
            "Scenarios,\n" +
            "Kerbals,\n" +
            "Groups";
    }
}
