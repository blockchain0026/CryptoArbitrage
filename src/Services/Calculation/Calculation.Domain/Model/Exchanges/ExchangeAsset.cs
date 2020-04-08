using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges
{
    public class ExchangeAsset : Entity
    {
        public int ExchangeId { get; private set; }
        public string Symbol { get; private set; }
        private decimal AvailableBalances { get; set; }
        public ExchangeAsset(int exchangeId, string currency, decimal balance = 0)
        {
            ExchangeId = exchangeId >= 0 ? exchangeId : throw new ArgumentOutOfRangeException(nameof(exchangeId));
            Symbol = !string.IsNullOrWhiteSpace(currency) ? currency : throw new ArgumentNullException(nameof(currency));
            AvailableBalances = balance;
        }
        public void UpdateBalance(decimal balance)
        {
            AvailableBalances = balance >= 0 ? balance : throw new ArgumentOutOfRangeException(nameof(balance));
        }

        public decimal GetAvailableBalances()
        {
            return AvailableBalances;
        }
    }
}
