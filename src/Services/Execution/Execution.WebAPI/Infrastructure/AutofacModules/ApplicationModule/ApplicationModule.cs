using Autofac;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using CryptoArbitrage.Services.Execution.Infrastructure.Idempotency;
using CryptoArbitrage.Services.Execution.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.WebAPI.Infrastructure.AutofacModules.ApplicationModule
{
    public class ApplicationModule : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;
        }

        protected override void Load(ContainerBuilder builder)
        {
            /*builder.Register(c => new OrderQueries(QueriesConnectionString))
                       .As<IOrderQueries>()
                       .InstancePerLifetimeScope();*/

            builder.RegisterType<SimpleArbitrageRepository>()
                .As<ISimpleArbitrageRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StopLossSettingRepository>()
                .As<IStopLossSettingRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();

            /*builder.RegisterAssemblyTypes(typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>)); */

        }
    }
}
