using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Markets
{
    public class MarketOrder : ValueObject
    {
        public int ExchangeId { get; private set; }
        public int Index { get; private set; }
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal TradingFee { get; private set; }
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }

        private MarketOrder(int exchangeId, int index, string baseCurrency, string quoteCurrency, decimal price, decimal quantity, decimal tradingFee)
        {
            this.ExchangeId = exchangeId;
            this.Index = index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof(index));
            this.BaseCurrency = baseCurrency;
            this.QuoteCurrency = quoteCurrency;
            this.Price = price;
            this.Quantity = quantity;
            this.TradingFee = tradingFee;
        }

        protected MarketOrder(string baseCurrency, string quoteCurrency, decimal price, decimal quantity)
        {
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            //Quantity = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity));
            if (quantity <0)
                throw new ArgumentOutOfRangeException(nameof(quantity));
            Quantity = quantity;
        }

        public MarketOrder(string baseCurrency, string quoteCurrency, decimal price, decimal quantity, Exchange exchange)
            : this(baseCurrency, quoteCurrency, price, quantity)
        {
            ExchangeId = exchange != null ? exchange.ExchangeId : throw new ArgumentNullException(nameof(exchange));

            var takerFee = exchange.TakerFee;
            TradingFee = takerFee >= 0 && takerFee < 1 ? takerFee : throw new ArgumentOutOfRangeException(nameof(exchange.TakerFee));
        }

        public MarketOrder(int exchangeId, decimal tradingFee, string baseCurrency, string quoteCurrency, decimal price, decimal quantity)
            : this(baseCurrency, quoteCurrency, price, quantity)
        {
            this.ExchangeId = exchangeId >= 0 ? exchangeId : throw new ArgumentOutOfRangeException(nameof(exchangeId));
            TradingFee = tradingFee >= 0 && tradingFee < 1 ? tradingFee : throw new ArgumentOutOfRangeException(nameof(tradingFee));
        }


        public MarketOrder ChangeIndex(int index)
        {
            return new MarketOrder(
                this.ExchangeId,
                index,
                this.BaseCurrency,
                this.QuoteCurrency,
                this.Price,
                this.Quantity,
                this.TradingFee
                );
        }

        public int IsBuyingCostLessThan(MarketOrder y)
        {
            var x = this;
            /*if (x == null)
                throw new ArgumentNullException(nameof(x));*/
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            var xMaxBuyingAmounts = x.Quantity * (1 - x.TradingFee);
            var yMaxBuyingAmounts = y.Quantity * (1 - y.TradingFee);

            var minAmountsToBuy = xMaxBuyingAmounts > yMaxBuyingAmounts ? yMaxBuyingAmounts : xMaxBuyingAmounts;


            var xCost = x.Price * minAmountsToBuy / (1 - x.TradingFee) / x.Quantity;
            var yCost = y.Price * minAmountsToBuy / (1 - y.TradingFee) / y.Quantity;

            if (xCost < yCost)
                return 1;
            else if (xCost > yCost)
                return -1;
            else
                return 0;
        }

        public int IsSellingProfitsMoreThan(MarketOrder y)
        {
            var x = this;
            /*if (x == null)
                throw new ArgumentNullException(nameof(x));*/
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            var maxAmountsToSell = x.Quantity > y.Quantity ? y.Quantity : x.Quantity;

            var xProfits = x.Price * maxAmountsToSell * (1 - x.TradingFee);
            var yProfits = y.Price * maxAmountsToSell * (1 - y.TradingFee);

            if (xProfits > yProfits)
                return 1;
            else if (xProfits < yProfits)
                return -1;
            else
                return 0;

            //return xProfits >= yProfits;
        }

        /*public decimal CalculateTradingFee()
        {

        }*/


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ExchangeId;
            yield return Price;
            yield return Quantity;
            yield return TradingFee;
        }
    }
}
