using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Umbraco.Core.Models.PublishedContent;

namespace YuzuDelivery.Umbraco.Core
{

    public class ImageMappings : Profile
    {
        public ImageMappings()
        {

            CreateMap<IPublishedContent, vmBlock_DataImage>()
                .ConvertUsing<ImageConvertor>();
        }
    }

    public class ImageConvertor : ITypeConverter<IPublishedContent, vmBlock_DataImage>
    {
        public vmBlock_DataImage Convert(IPublishedContent source, vmBlock_DataImage destination, ResolutionContext context)
        {
            if(source != null && source.ContentType != null)
            {
                if (source.ContentType.Alias == "Image")
                    return CreateImage(source);
                if (source.ContentType.Alias == "File")
                    return CreateFile(source);
            }
            return new vmBlock_DataImage();
        }

        public vmBlock_DataImage CreateImage(IPublishedContent image)
        {
            if (image != null)
            {
                return new vmBlock_DataImage
                {
                    Src = image.Url,
                    Alt = string.Empty
                };
            }
            return new vmBlock_DataImage();
        }

        public vmBlock_DataImage CreateFile(IPublishedContent image)
        {
            if (image != null)
            {
                return new vmBlock_DataImage
                {
                    Src = image.Url,
                    Alt = string.Empty
                };
            }
            return new vmBlock_DataImage();
        }
    }

}
