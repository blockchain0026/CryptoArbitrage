using CryptoArbitrage.Services.Calculation.Domain.Model.Markets;
using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Profits
{
    public class SimpleArbitrage : ValueObject
    {
        public MarketOrder BuyOrder { get; private set; }
        public MarketOrder SellOrder { get; private set; }
        public Decimal EstimatedProfits { get; private set; }


        public SimpleArbitrage(MarketOrder buyOrder, MarketOrder sellOrder, decimal estimatedProfits)
        {
            BuyOrder = buyOrder ?? throw new ArgumentNullException(nameof(buyOrder));
            SellOrder = sellOrder ?? throw new ArgumentNullException(nameof(sellOrder));
#if DEBUG
            EstimatedProfits = estimatedProfits;
#else
                     EstimatedProfits = estimatedProfits > 0 ? estimatedProfits : throw new ArgumentOutOfRangeException(
                $"The estimated profits {estimatedProfits} must higher than 0."
                );
#endif

        }

        /*public static SimpleArbitrage FromOrders(IEnumerable<MarketOrder> buyOrders, IEnumerable<MarketOrder> sellOrders)
        {

            

        }*/

        public SimpleArbitrage TakeBalanceIntoConsideration(decimal buyOrderExchangeQuoteCurrencyBalance, decimal sellOrderExchangeBaseCurrencyBalance)
        {
            var marketBuyOrder = new MarketOrder(
                this.BuyOrder.ExchangeId,
                this.BuyOrder.TradingFee,
                this.BuyOrder.BaseCurrency,
                this.BuyOrder.QuoteCurrency,
                this.BuyOrder.Price,
                buyOrderExchangeQuoteCurrencyBalance <= 0 ? 0 : buyOrderExchangeQuoteCurrencyBalance*0.995M / this.BuyOrder.Price
                );
            var marketSellOrder = new MarketOrder(
                this.SellOrder.ExchangeId,
                this.SellOrder.TradingFee,
                this.SellOrder.BaseCurrency,
                this.SellOrder.QuoteCurrency,
                this.SellOrder.Price,
                sellOrderExchangeBaseCurrencyBalance*0.995M
                );

            decimal room;



            var maxAmountsToBuy = marketBuyOrder.Quantity > marketSellOrder.Quantity ? marketSellOrder.Quantity : marketBuyOrder.Quantity;

            // The amounts of base currency received must be calculated with the charging fee.
            var receivingBaseCurrencyAmount = maxAmountsToBuy - maxAmountsToBuy * marketBuyOrder.TradingFee;

            // The amounts of base currency we lose which we selled is the amount we get from executed buy order.
            var losingBaseCurrencyAmount = receivingBaseCurrencyAmount;

            // The amounts of quote currency we get are equal to the selling profit which are calculated with charging fees.
            var receivingQuoteCurrencyAmount = marketSellOrder.Price * losingBaseCurrencyAmount * (1 - marketSellOrder.TradingFee);

            // The amounts of quote currency we lost are equal to the amounts that using to exchange base currency.
            var losingQuoteCurrencyAmount = marketBuyOrder.Price * maxAmountsToBuy;


            room = receivingQuoteCurrencyAmount - losingQuoteCurrencyAmount;

            var arbitrageBuyOrder = new MarketOrder(
                 marketBuyOrder.ExchangeId,
                 marketBuyOrder.TradingFee,
                 marketBuyOrder.BaseCurrency,
                 marketBuyOrder.QuoteCurrency,
                 marketBuyOrder.Price,
                 maxAmountsToBuy
                 );
            var arbitrageSellOrder = new MarketOrder(
                marketSellOrder.ExchangeId,
                marketSellOrder.TradingFee,
                marketSellOrder.BaseCurrency,
                marketSellOrder.QuoteCurrency,
                marketSellOrder.Price,
                losingBaseCurrencyAmount
                );

            var simpleArbitrage = new SimpleArbitrage(arbitrageBuyOrder, arbitrageSellOrder, room);

            /*Debug.WriteLine(
                "Time:" + DateTime.UtcNow.ToShortTimeString() + "\n"
                            + "Market:" + simpleArbitrage.BuyOrder.BaseCurrency + simpleArbitrage.BuyOrder.QuoteCurrency + "\n"
                            + "Buy From:" + "ExchangeId => " + simpleArbitrage.BuyOrder.ExchangeId + "\n"
                            + "Buy Price:" + simpleArbitrage.BuyOrder.Price + "\n"
                            + "Buy Amount:" + simpleArbitrage.BuyOrder.Quantity + "\n"
                            + "Sell To:" + "ExchangeId => " + simpleArbitrage.SellOrder.ExchangeId + "\n"
                            + "Sell Price:" + simpleArbitrage.SellOrder.Price + "\n"
                            + "Sell Amount:" + simpleArbitrage.SellOrder.Quantity + "\n"
                            + "Estimated Profits:" + simpleArbitrage.EstimatedProfits + "\n"
                );*/

            return simpleArbitrage;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.BuyOrder;
            yield return this.SellOrder;
            yield return this.EstimatedProfits;
        }
    }
}
