using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace CryptoArbitrage.Services.Execution.Domain.Model.StopLoss
{
    public class SlipPrice : Entity
    {
        public const decimal DefaultSlipPercents= 0.005M;
        public const decimal DefaultSlipQuantity = 0;
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal SlipQuantity { get; private set; }
        public decimal SlipPercents { get; private set; }
        public SlipPrice(string baseCurrency, string quoteCurrency, decimal slipQuantity, decimal slipPercents)
        {
            this.BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            this.QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            this.SlipQuantity = slipQuantity;
            this.SlipPercents = slipPercents < 1 ? slipPercents : throw new ArgumentOutOfRangeException(nameof(slipPercents));
        }
    }
}