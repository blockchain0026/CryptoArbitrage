using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.EventBus.Events;
using CryptoArbitrage.IntegrationEventLogEF.Services;
using CryptoArbitrage.IntegrationEventLogEF.Utilities;
using CryptoArbitrage.Services.Execution.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents
{

    public class ExecutionIntegrationEventService : IExecutionIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly ExecutionContext _executionContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public ExecutionIntegrationEventService(IEventBus eventBus, ExecutionContext executionContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_executionContext.Database.GetDbConnection());
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            await SaveEventAndOrderingContextChangesAsync(evt);
            _eventBus.Publish(evt);
            //await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        private async Task SaveEventAndOrderingContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_executionContext)
                .ExecuteAsync(async () => {
                    // Achieving atomicity between original ordering database operation and the IntegrationEventLog thanks to a local transaction
                    await _executionContext.SaveChangesAsync();
                    //await _eventLogService.SaveEventAsync(evt, _executionContext.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}
