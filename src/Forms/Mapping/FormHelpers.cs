using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq.Expressions;

namespace YuzuDelivery.Umbraco.Forms
{
    public static class GridMappingsService
    {
        public static void AddForm<TSource, TDest>(this Profile profile, Expression<Func<TSource, object>> sourceMember, Expression<Func<TDest, vmBlock_DataForm>> destMember)
        {
            profile.CreateMap<TSource, TDest>()
                .ForMember(destMember, opt => opt.MapFrom<FormMemberValueResolver<TSource, TDest>, object>(sourceMember));
        }
    }
}
