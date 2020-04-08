using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Events
{
    public class OrderCanceledDomainEvent : INotification
    {
        public OrderCanceledDomainEvent(string orderId, string arbitrageId, Exchange exchange, string exchangeOrderId,
            string baseCurrency, string quoteCurrency, decimal price, decimal quantityTotal, decimal quantityFilled, DateTime dateCanceled)
        {
            OrderId = orderId;
            ArbitrageId = arbitrageId;
            Exchange = exchange;
            ExchangeOrderId = exchangeOrderId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Price = price;
            QuantityTotal = quantityTotal;
            QuantityFilled = quantityFilled;
            DateCanceled = dateCanceled;
        }

        public string OrderId { get; private set; }
        public string ArbitrageId { get; private set; }
        public Exchange Exchange { get; private set; }
        public string ExchangeOrderId { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal QuantityTotal { get; private set; }
        public decimal QuantityFilled { get; private set; }
        public DateTime DateCanceled { get; private set; }
    }
}
