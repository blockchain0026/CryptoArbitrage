using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents
{
    public interface IExecutionIntegrationEventService
    {
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
