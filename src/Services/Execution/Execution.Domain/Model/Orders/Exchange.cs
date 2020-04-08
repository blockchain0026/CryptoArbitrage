using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Orders
{
    public class Exchange : ValueObject
    {
        public int ExchangeId { get; private set; }
        public Exchange(int exchangeId)
        {
            this.ExchangeId = exchangeId > 0 ? exchangeId : throw new ArgumentNullException(nameof(exchangeId));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.ExchangeId;
        }
    }
}
