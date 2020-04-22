using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class YuzuPropertyFactoryMapperSettings : YuzuMapperSettings
    {
        public Type Factory { get; set; }
        public Type Source { get; set; }
        public Type Dest { get; set; }
        public string DestPropertyName { get; set; }
        public string GroupName { get; set; }
    }
}
