using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using System.Reflection;
using AutoMapper;
using Umbraco.Core;

namespace YuzuDelivery.Umbraco.Core
{
    public static class CompositionExtensions
    {
        public static void RegisterAll<T>(this Composition composition, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => x.GetInterfaces().Any(y => y == typeof(T)));
            foreach (var f in types)
            {
                composition.Register(typeof(T), f);
            }
        }

        public static void RegisterYuzuMapping(this Composition composition, Assembly assembly)
            => composition.Register<IMapper>((factory) =>
            {
                return factory.GetInstance<DefaultUmbracoMappingFactory>().Create(assembly, factory);
            });
    }
}
