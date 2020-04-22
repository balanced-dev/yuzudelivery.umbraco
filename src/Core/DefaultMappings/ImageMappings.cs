using System;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Models.PublishedContent;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{

    public class ImageMappings : YuzuMappingConfig
    {
        public ImageMappings()
        {
            ManualMaps.AddTypeReplace<ImageConvertor>(false);
        }
    }

    public class ImageConvertor : IYuzuTypeConvertor<IPublishedContent, vmBlock_DataImage>
    {
        public vmBlock_DataImage Convert(IPublishedContent source, UmbracoMappingContext context)
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
