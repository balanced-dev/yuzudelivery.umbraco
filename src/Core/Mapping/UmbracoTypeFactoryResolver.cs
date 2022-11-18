using System.Collections.Generic;
using YuzuDelivery.Core;
using YuzuDelivery.Core.Mapping;

namespace YuzuDelivery.Umbraco.Core.Mapping
{
    public class UmbracoTypeFactoryRunner : IYuzuTypeFactoryRunner
    {
        private readonly IYuzuConfiguration config;
        private readonly IMappingContextFactory contextFactory;

        public UmbracoTypeFactoryRunner(IYuzuConfiguration config, IMappingContextFactory contextFactory)
        {
            this.config = config;
            this.contextFactory = contextFactory;
        }

        public E Run<E>(IDictionary<string, object> items = null)
        {
            if(config.ViewmodelFactories.ContainsKey(typeof(E)))
            {
                var typeFactory = config.ViewmodelFactories[typeof(E)]() as IYuzuTypeFactory<E>;

                if (typeFactory != null)
                {
                    if (items == null) items = new Dictionary<string, object>();
                    return typeFactory.Create(contextFactory.Create<UmbracoMappingContext>(items));
                }
            }

            return default(E);
        }

    }
}
