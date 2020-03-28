using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class YuzuTypeMapperSettings : YuzuMapperSettings
    {
        public Type Convertor { get; set; }
        public bool IgnoreReturnType { get; set; }
    }
}
