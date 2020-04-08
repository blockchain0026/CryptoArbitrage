using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events
{
    public class ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent : IntegrationEvent
    {
        public ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent(string arbitrageOrderId, int exchangeId, string baseCurrency, string quoteCurrency, decimal quantity, decimal price)
        {
            ArbitrageOrderId = arbitrageOrderId;
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Quantity = quantity;
            Price = price;
        }

        public string ArbitrageOrderId { get; private set; }
        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
    }
}
