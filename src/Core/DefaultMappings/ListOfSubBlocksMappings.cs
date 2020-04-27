﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;
using Umbraco.Web.Composing;
using Umbraco.Core;
using YuzuDelivery.Core;
using YuzuDelivery.Umbraco.Import;

namespace YuzuDelivery.Umbraco.Core
{
    public class ListOfSubBlocksMappings : YuzuMappingConfig
    {
        public ListOfSubBlocksMappings(IVmPropertyMappingsFinder vmPropertyMappingsFinder)
        {
            var listOfObjectsMappings = vmPropertyMappingsFinder.GetMappings<object>();

            foreach (var i in listOfObjectsMappings)
            {
                if (i.SourceType != null)
                {
                    var resolverType = typeof(SubBlocksObjectResolver<,>).MakeGenericType(i.SourceType, i.DestType);

                    ManualMaps.Add(new YuzuFullPropertyMapperSettings()
                    {
                        Mapper = typeof(IYuzuFullPropertyMapper),
                        Resolver = resolverType,
                        SourcePropertyName = i.SourcePropertyName,
                        DestPropertyName = i.DestPropertyName
                    });
                }
            }
        }
    }

    public class SubBlocksObjectResolver<Source, Dest> : IYuzuFullPropertyResolver<Source, Dest, IPublishedContent, object>
    {
        private readonly IMapper mapper;
        private readonly IYuzuConfiguration config;

        public SubBlocksObjectResolver(IMapper mapper, IYuzuConfiguration config)
        {
            this.mapper = mapper;
            this.config = config;
        }

        public object Resolve(Source source, Dest destination, IPublishedContent sourceMember, string destPropertyName, UmbracoMappingContext context)
        {
            var cmsModel = config.CMSModels.Where(c => c.Name == sourceMember.ContentType.Alias.FirstCharacterToUpper()).FirstOrDefault();
            var viewmodel = config.ViewModels.Where(x => x.Name == $"{YuzuConstants.Configuration.BlockPrefix}{cmsModel.Name}" || x.Name == $"{YuzuConstants.Configuration.PagePrefix}{cmsModel.Name}").FirstOrDefault();

            return mapper.Map(sourceMember, cmsModel, viewmodel, context.Items);
        }
    }

}
