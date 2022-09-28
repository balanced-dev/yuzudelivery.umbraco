using System;
using System.Collections.Generic;
using Umb = Umbraco.Cms.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace YuzuDelivery.Umbraco.Core
{
    public interface IFactory
    {
        object GetInstance(Type type);

        TService GetInstance<TService>() where TService : class;

        IEnumerable<TService> GetAllInstances<TService>() where TService : class;
    }

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
}
