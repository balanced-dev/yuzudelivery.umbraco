using System;
using System.Collections.Generic;

#if NETCOREAPP 
using Umb = Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
#else
using Umb = Umbraco.Core.Composing;
using Umbraco.Core;
#endif

namespace YuzuDelivery.Umbraco.Core
{
    public interface IFactory
    {
        object GetInstance(Type type);

        TService GetInstance<TService>() where TService : class;

        IEnumerable<TService> GetAllInstances<TService>() where TService : class;
    }

#if NETCOREAPP
    public class Umb9Factory : IFactory
    {
        private readonly IServiceProvider _factory;
        public Umb9Factory(IServiceProvider factory)
        {
            _factory = factory;
        }

        public object GetInstance(Type type)
        {
            return _factory.GetService(type);
        }

        public TService GetInstance<TService>() where TService : class
        {
            return _factory.GetService<TService>();
        }

        public IEnumerable<TService> GetAllInstances<TService>() where TService : class
        {
            return _factory.GetServices<TService>();
        }
    }
#else
    public class Umb8Factory : IFactory
    {
        private readonly Umb.IFactory _factory;
        public Umb8Factory(Umb.IFactory factory)
        {
            _factory = factory;
        }

        public object GetInstance(Type type)
        {
            return _factory.GetInstance(type);
        }

        public TService GetInstance<TService>()
            where TService : class
        {
            return _factory.GetInstance<TService>();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
            where TService : class
        {
            return _factory.GetAllInstances<TService>();
        }
    }
#endif
}
