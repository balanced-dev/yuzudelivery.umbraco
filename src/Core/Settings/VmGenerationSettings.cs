using System.ComponentModel;

namespace YuzuDelivery.Umbraco.Core.Settings
{
    public class VmGenerationSettings
    {
        internal const bool StaticIsActive = true;
        internal const string StaticDirectory = "./Yuzu/ViewModels";

        [DefaultValue(StaticIsActive)]
        public bool IsActive { get; set; } = StaticIsActive;

        [DefaultValue(StaticDirectory)]
        public bool AcceptUnsafeDirectory { get; set; }

        public string Directory { get; set; } = StaticDirectory;
    }
}
