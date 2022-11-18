using Microsoft.CodeAnalysis.Diagnostics;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using YuzuDelivery.Core;

#if NETCOREAPP
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
#endif

namespace YuzuDelivery.Umbraco.Core
{

    public class ImageMappings : YuzuMappingConfig
    {
        public ImageMappings()
        {
            ManualMaps.AddTypeReplace<ImageConvertor>(false);
            ManualMaps.AddTypeReplace<MediWithCropsConvertor>(false);
        }
    }

    public class MediWithCropsConvertor : IYuzuTypeConvertor<MediaWithCrops, vmBlock_DataImage>
    {
        private ImageFactory imageFactory;

        public MediWithCropsConvertor(ImageFactory imageFactory)
        {
            this.imageFactory = imageFactory;
        }

        public vmBlock_DataImage Convert(MediaWithCrops source, UmbracoMappingContext context)
        {
            if (source != null && source.ContentType != null)
            {
                if (source.ContentType.Alias == "Image" || source.ContentType.Alias == "umbracoMediaVectorGraphics")
                    return imageFactory.CreateImage(source.Content);
                if (source.ContentType.Alias == "File")
                    return imageFactory.CreateFile(source.Content);
            }
            return new vmBlock_DataImage();
        }
    }

    public class ImageConvertor : IYuzuTypeConvertor<IPublishedContent, vmBlock_DataImage>
    {
        private ImageFactory imageFactory;

        public ImageConvertor(ImageFactory imageFactory)
        {
            this.imageFactory = imageFactory;
        }

        public vmBlock_DataImage Convert(IPublishedContent source, UmbracoMappingContext context)
        {
            if(source != null && source.ContentType != null)
            {
                if (source.ContentType.Alias == "Image")
                    return imageFactory.CreateImage(source);
                if (source.ContentType.Alias == "File")
                    return imageFactory.CreateFile(source);
            }
            return new vmBlock_DataImage();
        }
    }

    public class ImageFactory
    {
        public vmBlock_DataImage CreateImage(IPublishedContent image)
        {
            if (image != null)
            {
                var mapped = new vmBlock_DataImage
                {
                    Src = image.Url(),
                    Alt = image.Value<string>("alt"),
                    Height = image.Value<int>("umbracoHeight"),
                    Width = image.Value<int>("umbracoWidth"),
                    FileSize = image.Value<int>("umbracoBytes"),
                    Extension = image.Value<string>("umbracoExtension")
                };
                if (image.HasValue("umbracoFile") && image.Value("umbracoFile") is ImageCropperValue icv)
                {
                    if (icv.FocalPoint != null)
                    {
                        mapped.FocalPointLeft = icv.FocalPoint.Left.ToString();
                        mapped.FocalPointTop = icv.FocalPoint.Top.ToString();
                    }
                }

                return mapped;
            }
            return new vmBlock_DataImage();
        }

        public vmBlock_DataImage CreateFile(IPublishedContent image)
        {
            if (image != null)
            {
                return new vmBlock_DataImage
                {
                    Src = image.Url(),
                    Alt = image.Value<string>("alt"),
                    Height = image.Value<int>("umbracoHeight"),
                    Width = image.Value<int>("umbracoWidth"),
                    FileSize = image.Value<int>("umbracoBytes")
                };
            }
            return new vmBlock_DataImage();
        }
    }

}
