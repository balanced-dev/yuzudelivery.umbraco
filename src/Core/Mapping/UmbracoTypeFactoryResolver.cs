using System.Collections.Generic;
using Microsoft.Extensions.Options;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public class UmbracoTypeFactoryRunner : IYuzuTypeFactoryRunner
    {
        private readonly IOptions<YuzuConfiguration> config;
        private readonly IMappingContextFactory<UmbracoMappingContext> contextFactory;

        public UmbracoTypeFactoryRunner(IOptions<YuzuConfiguration> config, IMappingContextFactory<UmbracoMappingContext> contextFactory)
        {
            this.config = config;
            this.contextFactory = contextFactory;
        }

        public E Run<E>(IDictionary<string, object> items = null)
        {
            if(config.Value.ViewmodelFactories.ContainsKey(typeof(E)))
            {
                var typeFactory = config.Value.ViewmodelFactories[typeof(E)]() as IYuzuTypeFactory<E>;

                if (typeFactory != null)
                {
                    if (items == null) items = new Dictionary<string, object>();
                    return typeFactory.Create(contextFactory.Create(items));
                }
            }

            return default(E);
        }

    }
}
