using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events
{
    public class TimeForUpdateBalanceIntegrationEvent : IntegrationEvent
    {
        public int ExchangeId { get; private set; }
        public TimeForUpdateBalanceIntegrationEvent(int exchangeId)
        {
            this.ExchangeId = exchangeId;
        }
    }
}
