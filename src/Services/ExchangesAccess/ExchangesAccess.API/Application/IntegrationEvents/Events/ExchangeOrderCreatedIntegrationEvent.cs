using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events
{
    public class ExchangeOrderCreatedIntegrationEvent:IntegrationEvent
    {
        public ExchangeOrderCreatedIntegrationEvent(int exchangeId, string exchangeOrderId,string baseCurrency,string quoteCurrency,decimal quantity,decimal price)
        {
            ExchangeId = exchangeId;
            ExchangeOrderId = exchangeOrderId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Quantity = quantity;
            Price = price;
        }

        public int ExchangeId { get; private set; }
        public string ExchangeOrderId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Price { get; private set; }
    }
}
