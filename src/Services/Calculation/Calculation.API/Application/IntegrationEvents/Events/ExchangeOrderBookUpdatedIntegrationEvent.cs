using CryptoArbitrage.EventBus.Events;
using CryptoArbitrage.Services.Calculation.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.IntegrationEvents.Events
{
    public class ExchangeOrderBookUpdatedIntegrationEvent : IntegrationEvent
    {
        public ExchangeOrderBookUpdatedIntegrationEvent(int exchangeId, string baseCurrency, string quoteCurrency, OrderBookViewModel orderBook)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            OrderBook = orderBook;
        }

        public int ExchangeId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }

        public OrderBookViewModel OrderBook { get; private set; }


    }
}
