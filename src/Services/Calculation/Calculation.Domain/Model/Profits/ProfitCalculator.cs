using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using CryptoArbitrage.Services.Calculation.Domain.Model.Markets;
using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Profits
{
    public class ProfitCalculator : DomainService
    {
        private const decimal MIN_ROOM_OF_PROFIT = 0;
        private const decimal SLIP_PRICE = 0;
       

        public ProfitCalculator(/*IMarketRepository marketRepository, IExchangeRepository exchangeRepository*/)
        {
            /*_marketRepository = marketRepository != null ? marketRepository : throw new ArgumentNullException(nameof(marketRepository));
            _exchangeRepository = exchangeRepository != null ? exchangeRepository : throw new ArgumentNullException(nameof(exchangeRepository));*/
        }

        public static Task<bool> FindProfits(out IEnumerable<SimpleArbitrage> simpleArbitrages, string baseCurrency, string quoteCurrency, Market market,int? exchangeId=null)
        {
            try
            {
                simpleArbitrages = null;
                if (String.IsNullOrEmpty(baseCurrency) || String.IsNullOrEmpty(quoteCurrency))
                {
                    throw new ArgumentNullException(nameof(baseCurrency) + ", " + nameof(quoteCurrency));
                }

                if (market == null)
                    throw new KeyNotFoundException($"Could not find market with symbol {market}.");
                //var arbitrageOrder = market.GenerateArbitrageOrder();
                //var arbitrageOrder = SimpleArbitrage.FromOrders(market.BuyOrders, market.SellOrders);


#if DEBUG

                if (CalculateProfits(out IEnumerable<SimpleArbitrage> foundedSimpleArbitrages, market.BuyOrders, market.SellOrders, exchangeId))
                {
                    simpleArbitrages = foundedSimpleArbitrages;
                    return Task.FromResult(true);
                }

                else
                    return Task.FromResult(false);
#else
              if (CalculateProfits(out SimpleArbitrage foundedSimpleArbitrage, market.BuyOrders, market.SellOrders))
            {

                /*var buyFromExchange = await this._exchangeRepository.GetAsync(simpleArbitrage.BuyOrder.ExchangeId);
                var sellToExchange = await this._exchangeRepository.GetAsync(simpleArbitrage.SellOrder.ExchangeId);*/

                //var arbitrageOrderWithBalanceCalculation = simpleArbitrage.TakeBalanceIntoConsideration(buyFromExchange, sellToExchange.GetTradingFee());
            if (foundedSimpleArbitrage.EstimatedProfits > 0)
                {
                    simpleArbitrage = foundedSimpleArbitrage;

                    return Task.FromResult(true);
                }
                }
#endif




                return Task.FromResult(false);
            }
   
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }







        private static bool CalculateProfits(out IEnumerable<SimpleArbitrage> simpleArbitrages, IEnumerable<MarketOrder> buyOrders, IEnumerable<MarketOrder> sellOrders, int? exchangeId = null)
        {
            try
            {
                var founded = false;
                var simpleArbitrageList = new List<SimpleArbitrage>();
                simpleArbitrages = simpleArbitrageList.AsEnumerable();

                /*var marketBuyOrders = new List<ValuePair<MarketOrder>();
                var marketSellOrders = new List<MarketOrder>();*/
                var arbitragePairs = new List<KeyValuePair<MarketOrder, MarketOrder>>();


                if (exchangeId == null)
                {
                    var marketBuyOrder = sellOrders.FirstOrDefault();
                    var marketSellOrder = buyOrders.FirstOrDefault();
                    if (marketBuyOrder == null || marketSellOrder == null)
                        return false;
                    if (marketBuyOrder.QuoteCurrency != marketSellOrder.QuoteCurrency)
                        throw new InvalidOperationException("The quote currency between buy and sell orders must be equal.");
                    if (marketBuyOrder.BaseCurrency != marketSellOrder.BaseCurrency)
                        throw new InvalidOperationException("The base currency between buy and sell orders must be equal.");
                    arbitragePairs.Add(new KeyValuePair<MarketOrder, MarketOrder>(
                        marketBuyOrder,
                        marketSellOrder
                        ));
                }
                else
                {
                    var exchangeBuyOrder = buyOrders.Where(o => o.ExchangeId == exchangeId).SingleOrDefault();
                    var exchangeSellOrder = sellOrders.Where(o => o.ExchangeId == exchangeId).SingleOrDefault();
                    if (exchangeBuyOrder == null && exchangeSellOrder == null)
                        return false;

                    if (exchangeBuyOrder != null)
                    {
                        foreach (var sellOrder in sellOrders)
                        {
                            arbitragePairs.Add(new KeyValuePair<MarketOrder, MarketOrder>(
                                sellOrder,
                                exchangeBuyOrder
                                ));
                        }
                    }
                    if (exchangeSellOrder != null)
                    {
                        foreach (var buyOrder in buyOrders)
                        {
                            arbitragePairs.Add(new KeyValuePair<MarketOrder, MarketOrder>(
                                exchangeSellOrder,
                                buyOrder
                                ));
                        }
                    }
                }


                foreach (var pair in arbitragePairs)
                {
                    var marketBuyOrder = pair.Key;
                    var marketSellOrder = pair.Value;

                    decimal room;

                    var maxAmountsToBuy = marketBuyOrder.Quantity > marketSellOrder.Quantity ? marketSellOrder.Quantity : marketBuyOrder.Quantity;

                    // The amounts of base currency received must calculate the charging fee.
                    var receivingBaseCurrencyAmount = maxAmountsToBuy - maxAmountsToBuy * marketBuyOrder.TradingFee;

                    // The amounts of base currency we lose which we selled is the amount we get from excuted buy order.
                    var losingBaseCurrencyAmount = receivingBaseCurrencyAmount;

                    // The amounts of quote currency we get are equal to the selling profit which are calculated with charging fee too.
                    var receivingQuoteCurrencyAmount = marketSellOrder.Price * losingBaseCurrencyAmount * (1 - marketSellOrder.TradingFee);

                    // The amounts of quote currency we lost are equal to the amounts that using to exchange base currency.
                    var losingQuoteCurrencyAmount = marketBuyOrder.Price * maxAmountsToBuy;


                    room = receivingQuoteCurrencyAmount - losingQuoteCurrencyAmount;


#if DEBUG
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
                    if (room > 0)
                    {
                        simpleArbitrageList.Add(simpleArbitrage);
                        founded = true;
                    }


#else

             if (room > 0)
            {
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

                simpleArbitrage = new SimpleArbitrage(arbitrageBuyOrder, arbitrageSellOrder, room);

                return true;
            }
#endif

                }

                simpleArbitrages = simpleArbitrageList.AsEnumerable();

                return founded;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
