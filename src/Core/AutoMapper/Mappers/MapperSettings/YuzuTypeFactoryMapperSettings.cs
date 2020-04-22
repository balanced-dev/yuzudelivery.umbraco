using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class YuzuTypeFactoryMapperSettings : YuzuMapperSettings
    {
        public Type Factory { get; set; }
        public Type Dest { get; set; }
    }
}
