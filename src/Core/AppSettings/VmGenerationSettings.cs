using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace YuzuDelivery.Umbraco.Core
{
    public class VmGenerationSettings
    {
        internal const bool StaticIsActive = true;
        internal const string StaticDirectory = "~/yuzu/viewmodels";

        [DefaultValue(StaticIsActive)]
        public bool IsActive { get; set; } = StaticIsActive;

        [DefaultValue(StaticDirectory)]
        public bool AcceptUnsafeDirectory { get; set; }

        public string Directory { get; set; } = StaticDirectory;
    }
}
