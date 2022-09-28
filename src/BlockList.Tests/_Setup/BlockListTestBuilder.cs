using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Autofac;

namespace YuzuDelivery.Umbraco.BlockList.Tests
{
    public class BlockListTestBuilder : IIOCSetup
    {
        public ContainerBuilder Apply(ContainerBuilder builder)
        {
            builder.RegisterType<BlockListDataTypeFactory>();
            builder.RegisterType<BlockListDbModelFactory>();

            builder.RegisterType<BlockListGridCreationService>();
            builder.RegisterType<BlockListGridContentMapper>();
            builder.RegisterType<BlockListEditorCreationService>();

            builder.RegisterType<BlockListGridRowConfigToContent>();

            builder.RegisterType<BlockListContentMapper>();

            builder.Register((IComponentContext factory) =>
            {
                return Substitute.For<GuidFactory>();
            }).SingleInstance();

            return builder;
        }
    }

}
