using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges
{
    public class Exchange : Entity, IAggregateRoot
    {
        public int ExchangeId { get; private set; }
        public decimal MakerFee { get; private set; }
        public decimal TakerFee { get; private set; }

        private List<ExchangeAsset> _assets;
        public IEnumerable<ExchangeAsset> ExchangeAssets => _assets.AsReadOnly();

        protected Exchange()
        {
            _assets = new List<ExchangeAsset>();
        }

        public Exchange(int exchangeId, decimal makerFee, decimal takerFee) : this()
        {
            ExchangeId = exchangeId >= 0 ? exchangeId : throw new ArgumentOutOfRangeException(nameof(exchangeId));
            MakerFee = makerFee >= 0 ? makerFee : throw new ArgumentOutOfRangeException(nameof(makerFee));
            TakerFee = takerFee >= 0 ? takerFee : throw new ArgumentOutOfRangeException(nameof(takerFee));
        }

        public decimal GetTradingFee()
        {
            return this.TakerFee;
        }

        public void UpdateAssetBalance(string symbol, decimal balance)
        {
            var existingAsset = _assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                //throw new KeyNotFoundException($"There is no asset with symbol {symbol} found in exchange {this.ExchangeId}.");
                this.AddAsset(symbol, balance);

            }
            else
            {
                existingAsset.UpdateBalance(balance);

            }
        }

        public void AddAsset(string symbol, decimal balance = 0)
        {
            var existingAsset = _assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                this._assets.Add(new ExchangeAsset(this.ExchangeId, symbol, balance));
            }
        }

        public decimal GetAssetBalance(string symbol)
        {
            var existingAsset = _assets.Where(a => a.Symbol == symbol).SingleOrDefault();
            if (existingAsset == null)
            {
                return 0;
                
                //throw new KeyNotFoundException($"There is no asset with symbol {symbol} found in exchange {this.ExchangeId}.");
            }
            return existingAsset.GetAvailableBalances();
        }
    }
}
