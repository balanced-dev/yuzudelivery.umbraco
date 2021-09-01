using YuzuDelivery.Umbraco.Import;
using YuzuDelivery.Umbraco.Import.Tests.Integration;
using Autofac;
using Rhino.Mocks;

namespace YuzuDelivery.Umbraco.Grid.Test
{
    public class GridTestBuilder : IIOCSetup
    {
        public ContainerBuilder Apply(ContainerBuilder builder)
        {
            builder.RegisterType<GridSchemaCreationService>().As<IGridSchemaCreationService>();

            builder.Register<IDTGEService>((IComponentContext factory) =>
            {
                return MockRepository.GeneratePartialMock<DTGEService>(new object[] {
                    factory.Resolve<IVmHelperService>()
                });
            }).SingleInstance();

            return builder;
        }
    }

}
