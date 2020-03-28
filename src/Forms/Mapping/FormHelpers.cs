using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Core;

namespace YuzuDelivery.Umbraco.Forms
{
    public static class FormMappingsExtension
    {
        public static void AddForm<TSource, TDest>(this List<YuzuMapperSettings> resolvers, Expression<Func<TSource, object>> sourceMember, Expression<Func<TDest, vmBlock_DataForm>> destMember)
        {
            resolvers.Add(new YuzuFullPropertyMapperSettings()
            {
                Mapper = typeof(IYuzuFullPropertyMapper),
                Resolver = typeof(FormValueResolver<TSource, TDest>),
                SourcePropertyName = sourceMember.GetMemberName(),
                DestPropertyName = destMember.GetMemberName()
            });
        }
    }
}
