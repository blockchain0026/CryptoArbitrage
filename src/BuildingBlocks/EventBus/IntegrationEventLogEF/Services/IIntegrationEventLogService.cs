using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.IntegrationEventLogEF.Services
{
    public interface IIntegrationEventLogService
    {
        Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction);
        Task MarkEventAsPublishedAsync(IntegrationEvent @event);
    }
}
