using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YuzuDelivery.Umbraco.Core;
using YuzuDelivery.Umbraco.Forms;
using YuzuDelivery.Umbraco.Grid;
using YuzuDelivery.UmbracoModels;
using YuzuDelivery.ViewModels;
using AutoMapper.Configuration;

namespace $rootnamespace$
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class MappingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var cfg = new MapperConfigurationExpression();
            cfg.AddMaps(typeof(MappingComposer));
            cfg.AddMaps(typeof(YuzuStartup));
            cfg.AddMaps(typeof(YuzuFormsStartup));


            var mapperConfig = new MapperConfiguration(cfg);

            composition.Register<IMapper>(new Mapper(mapperConfig));
        }
    }

}
