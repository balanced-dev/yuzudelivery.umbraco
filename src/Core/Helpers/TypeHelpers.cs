using System;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core.Helpers
{
    public static class TypeHelpers
    {
        public static string GetBlockName(this Type type)
        {
            return type.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "par");
        }
        public static string GetModelName(this Type type)
        {
            return type.Name.Replace(YuzuConstants.Configuration.BlockPrefix, "");
        }
    }
}