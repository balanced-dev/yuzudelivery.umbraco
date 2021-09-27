using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuzuDelivery.Umbraco.Core
{
    public class VmGenerationSettings
    {
        public bool IsActive { get; set; }
        public bool AcceptUnsafeDirectory { get; set; }
        public string Directory { get; set; }
    }
}
