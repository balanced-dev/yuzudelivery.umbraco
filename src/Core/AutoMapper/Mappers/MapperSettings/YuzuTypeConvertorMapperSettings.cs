using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public class YuzuTypeConvertorMapperSettings : YuzuMapperSettings
    {
        public Type Convertor { get; set; }
        public bool IgnoreReturnType { get; set; }
    }
}
