using System.Reflection;
using YuzuDelivery.Umbraco.Forms;
using Microsoft.Extensions.DependencyInjection;
using YuzuDelivery.Core;

namespace YuzuDelivery.Umbraco.Core
{

    public static class CompositionExtensions
    {
        public static void RegisterFormStrategies(this IServiceCollection services, Assembly assembly)
        {
            services.RegisterAll<IFormFieldMappings>(assembly);
            services.RegisterAll<IFormFieldPostProcessor>(assembly);
        }
    }
}
