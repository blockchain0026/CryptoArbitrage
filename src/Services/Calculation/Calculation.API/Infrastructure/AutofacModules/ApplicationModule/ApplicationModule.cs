using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Infrastructure.AutofacModules.ApplicationModule
{
    public class ApplicationModule : Autofac.Module
    {
        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {

            /*builder.Register(c => new OrderQueries(QueriesConnectionString))
                .As<IOrderQueries>()
                .InstancePerLifetimeScope();*/

            /*builder.RegisterType<BuyerRepository>()
                .As<IBuyerRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository>()
                .InstancePerLifetimeScope();*/

            /*builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();*/

            /*builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));*/

        }
    }
}
