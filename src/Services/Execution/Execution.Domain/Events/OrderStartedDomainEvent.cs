using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Events
{
    public class OrderStartedDomainEvent : INotification
    {

        public string OrderId { get; private set; }
        public string ArbitrageId { get; private set; }
        public Exchange Exchange { get; private set; }
        public OrderType OrderType { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal QuantityTotal { get; private set; }
       

        public OrderStartedDomainEvent(string orderId, string arbitrageId, Exchange exchange, OrderType orderType, string baseCurrency, string quoteCurrency,decimal price, decimal quantityTotal)
        {
            OrderId = orderId;
            ArbitrageId = arbitrageId;
            Exchange = exchange;
            OrderType = orderType;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Price = price;
            QuantityTotal = quantityTotal;
        }

    }
}
