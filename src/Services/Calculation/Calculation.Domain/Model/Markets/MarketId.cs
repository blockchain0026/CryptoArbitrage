using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Markets
{
    public class MarketId : ValueObject
    {
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }


        public MarketId(string baseCurrency, string quoteCurrency)
        {
            BaseCurrency = !String.IsNullOrWhiteSpace(baseCurrency) ? baseCurrency : throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = !String.IsNullOrWhiteSpace(quoteCurrency) ? quoteCurrency : throw new ArgumentNullException(nameof(quoteCurrency));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.BaseCurrency;
            yield return this.QuoteCurrency;
        }
    }
}
