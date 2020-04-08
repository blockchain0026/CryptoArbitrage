using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Events
{
    public class SimpleArbitrageOpenedDomainEvent : INotification
    {
        public SimpleArbitrage SimpleArbitrage { get; private set; }

        public SimpleArbitrageOpenedDomainEvent(SimpleArbitrage simpleArbitrage)
        {
            SimpleArbitrage = simpleArbitrage;
        }
    }
}
