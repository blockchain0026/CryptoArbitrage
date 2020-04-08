using CryptoArbitrage.Services.Calculation.Domain.Model.Markets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Events
{
    public class ExchangeMarketUpdatedDomainEvent : INotification
    {
        public Market Market { get; private set; }
        public int ExchangeId { get; private set; }

        public ExchangeMarketUpdatedDomainEvent(Market market, int exchangeId)
        {
            this.Market = market;
            this.ExchangeId = exchangeId;
        }
    }
}
