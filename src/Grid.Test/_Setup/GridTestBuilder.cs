using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Autofac;

namespace YuzuDelivery.Umbraco.Grid.Test
{
    public class GridTestBuilder : IIOCSetup
    {
        public ContainerBuilder Apply(ContainerBuilder builder)
        {
            builder.RegisterType<GridSchemaCreationService>().As<IGridSchemaCreationService>();

            builder.Register<IDTGEService>((IComponentContext factory) =>
            {
                return Substitute.ForPartsOf<DTGEService>(factory.Resolve<IVmHelperService>(), factory.Resolve<MapPathAbstraction>());
            }).SingleInstance();

            return builder;
        }
    }

}
