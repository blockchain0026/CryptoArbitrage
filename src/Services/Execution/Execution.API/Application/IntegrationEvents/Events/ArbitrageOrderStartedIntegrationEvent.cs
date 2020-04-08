using CryptoArbitrage.EventBus.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events
{
    public class ArbitrageOrderStartedIntegrationEvent:IntegrationEvent
    {
        public string ArbitrageOrderId { get; private set; }
        public string ArbitrageId { get; private set; }
        public int ExchangeId { get; private set; }
        public string OrderType { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal QuantityTotal { get; private set; }


        public ArbitrageOrderStartedIntegrationEvent(string arbitrageOrderId, string arbitrageId, int exchangeId, string orderType, string baseCurrency, string quoteCurrency, decimal price, decimal quantityTotal)
        {
            ArbitrageOrderId = arbitrageOrderId;
            ArbitrageId = arbitrageId;
            ExchangeId = exchangeId;
            OrderType = orderType;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Price = price;
            QuantityTotal = quantityTotal;
        }
    }
}
