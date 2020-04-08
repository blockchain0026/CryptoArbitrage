using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Events
{
    public class OrderCreatedDomainEvent : INotification
    {

        public string OrderId { get; private set; }
        public string ArbitrageId { get; private set; }
        public Exchange Exchange { get; private set; }
        public string ExchangeOrderId { get; private set; }
        public DateTime DateCreated { get; private set; }


        public OrderCreatedDomainEvent(string orderId, string arbitrageId, Exchange exchange, string exchangeOrderId,DateTime dateCreated)
        {
            OrderId = orderId;
            ArbitrageId = arbitrageId;
            Exchange = exchange;
            ExchangeOrderId = exchangeOrderId;
            DateCreated = dateCreated;
        }

    }
}
