using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Umbraco.Forms.Web.Models;

namespace YuzuDelivery.Umbraco.Forms
{
    public class CommentsProfile : Profile
    {

        public CommentsProfile()
        {
            CreateMap<string, vmBlock_DataForm>()
                .ConvertUsing<FormTypeConvertor>();

            CreateMap<FormViewModel, vmBlock_DataFormBuilder>()
                .ConvertUsing<FormBuilderTypeConverter>();
        }

    }
}
