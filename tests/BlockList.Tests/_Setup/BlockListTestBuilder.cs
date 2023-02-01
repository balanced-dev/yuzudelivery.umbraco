using System;
using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Web;
using YuzuDelivery.Import.Settings;
using YuzuDelivery.Umbraco.BlockList.Grid.ContentCreation;
using YuzuDelivery.Umbraco.BlockList.Tests.Stubs;

namespace YuzuDelivery.Umbraco.BlockList.Tests
{
    public class BlockListTestBuilder : IIOCSetup
    {
        public ContainerBuilder Apply(ContainerBuilder builder)
        {
            builder.RegisterType<BlockListDataTypeFactory>();
            builder.RegisterType<BlockListDbModelFactory>();

            builder.RegisterType<BlockGridCreationService>();
            builder.RegisterType<BlockGridContentMapper>();

            builder.Register(c =>
            {
                var sp = c.Resolve<IServiceProvider>();
                var guidFactory = (GuidFactory) sp.GetRequiredService<ConsistentGuidFactoryStub>();
                var dbmodelFactory = ActivatorUtilities.CreateInstance<BlockListDbModelFactory>(sp, guidFactory);

                return ActivatorUtilities.CreateInstance<BlockGridContentMapper>(sp, guidFactory, dbmodelFactory);
            });


            builder.RegisterType<BlockListEditorCreationService>();

            builder.RegisterType<BlockGridRowConfigToContent>();

            builder.RegisterType<BlockListContentMapper>();

            builder.RegisterInstance(Substitute.For<IUmbracoContextAccessor>()).As<IUmbracoContextAccessor>();

            builder.Register((IComponentContext factory) =>
            {
                return Substitute.For<GuidFactory>();
            }).SingleInstance();

            builder.RegisterType<ConsistentGuidFactoryStub>().AsSelf().SingleInstance();


            return builder;
        }
    }
}
