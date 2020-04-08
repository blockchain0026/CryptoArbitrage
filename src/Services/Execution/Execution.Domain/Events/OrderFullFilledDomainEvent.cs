using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Events
{
    public class OrderFullFilledDomainEvent:INotification
    {
        public OrderFullFilledDomainEvent(string orderId, string arbitrageId, Exchange exchange, string exchangeOrderId, DateTime dateFilled)
        {
            OrderId = orderId;
            ArbitrageId = arbitrageId;
            Exchange = exchange;
            ExchangeOrderId = exchangeOrderId;
            DateFilled = dateFilled;
        }

        public string OrderId { get; private set; }
        public string ArbitrageId { get; private set; }
        public Exchange Exchange { get; private set; }
        public string ExchangeOrderId { get; private set; }
        public DateTime DateFilled { get; private set; }
    }
}
