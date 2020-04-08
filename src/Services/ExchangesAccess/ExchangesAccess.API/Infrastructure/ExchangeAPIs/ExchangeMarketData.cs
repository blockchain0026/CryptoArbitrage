using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public class ExchangeMarketData : IExchangeMarketData
    {
        private readonly IExchangeApiClient _exchangeApiClient;

        private Object sync = new object();
        private const int MAX_SIZE = 100;

        private readonly Dictionary<string, MarketData> _bittrex;
        private readonly Dictionary<string, MarketData> _binance;
        private readonly Dictionary<string, MarketData> _bitstamp;
        private readonly Dictionary<string, MarketData> _poloniex;
        private readonly Dictionary<string, MarketData> _kucoin;

        private Dictionary<string, List<MarketData>> _exchangeMarkets;

        /*public IEnumerable<MarketData> Bittrex => this._bittrex.AsEnumerable();
        public IEnumerable<MarketData> Binance => this._binance.AsEnumerable();
        public IEnumerable<MarketData> Bitstamp => this._bitstamp.AsEnumerable();
        public IEnumerable<MarketData> Poloniex => this._poloniex.AsEnumerable();*/


        public ExchangeMarketData(IExchangeApiClient exchangeApiClient)
        {
            _exchangeApiClient = exchangeApiClient;

            this._bittrex = new Dictionary<string, MarketData>();
            this._binance = new Dictionary<string, MarketData>();
            this._bitstamp = new Dictionary<string, MarketData>();
            this._poloniex = new Dictionary<string, MarketData>();
            this._kucoin = new Dictionary<string, MarketData>();
            //this._exchangeMarkets = new Dictionary<string, List<MarketData>>();
        }


        public async Task InitializeMarketData(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            try
            {
                if (exchangeId == "0")
                {
                    await Task.Run(() =>
                    {
                        lock (this.sync)
                        {

                            var marketKey = (baseCurrency + quoteCurrency).ToUpper();
                            var orderBook = _exchangeApiClient.GetOrderBook(exchangeId, baseCurrency, quoteCurrency).Result;
                            MarketData exist = null;

                            if (this._bittrex.TryGetValue(marketKey, out exist))
                            {
                                this._bittrex.Remove(marketKey);
                            }

                            if (orderBook.ErrorMessage == null && orderBook.Result != null)
                            {

                                var marketData = new MarketData
                                {
                                    Initialized = true,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = orderBook.Result
                                };

                                this._bittrex.TryAdd(marketKey, marketData);

                                var sizeLimitedMarketData = LimitSize("0", marketData);
                                this._bittrex[marketKey] = sizeLimitedMarketData;
                            }
                            else
                            {
                                this._bittrex.TryAdd(marketKey, new MarketData
                                {
                                    Initialized = false,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = new OrderBookViewModel
                                    {
                                        asks = new List<OrderPriceAndQuantityViewModel>(),
                                        bids = new List<OrderPriceAndQuantityViewModel>()
                                    }
                                });
                            }

                        }
                    });

                }
                else if (exchangeId == "1")
                {
                    await Task.Run(() =>
                    {
                        lock (this.sync)
                        {
                            var marketKey = (baseCurrency + quoteCurrency).ToUpper();
                            var orderBook = _exchangeApiClient.GetOrderBook(exchangeId, baseCurrency, quoteCurrency).Result;
                            MarketData exist = null;

                            if (this._binance.TryGetValue(marketKey, out exist))
                            {
                                this._binance.Remove(marketKey);
                            }

                            if (orderBook.ErrorMessage == null && orderBook.Result != null)
                            {

                                var marketData = new MarketData
                                {
                                    Initialized = true,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = orderBook.Result
                                };

                                this._binance.TryAdd(marketKey, marketData);

                                var sizeLimitedMarketData = LimitSize("0", marketData);
                                this._binance[marketKey] = sizeLimitedMarketData;
                            }
                            else
                            {
                                this._binance.TryAdd(marketKey, new MarketData
                                {
                                    Initialized = false,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = new OrderBookViewModel
                                    {
                                        asks = new List<OrderPriceAndQuantityViewModel>(),
                                        bids = new List<OrderPriceAndQuantityViewModel>()
                                    }
                                });
                            }


                        }
                    });
                }
                else if (exchangeId == "3")
                {
                    await Task.Run(() =>
                    {
                        lock (this.sync)
                        {
                            var marketKey = (baseCurrency + quoteCurrency).ToUpper();
                            var orderBook = _exchangeApiClient.GetOrderBook(exchangeId, baseCurrency, quoteCurrency).Result;
                            MarketData exist = null;

                            if (this._bitstamp.TryGetValue(marketKey, out exist))
                            {
                                this._bitstamp.Remove(marketKey);
                            }

                            if (orderBook.ErrorMessage == null && orderBook.Result != null)
                            {

                                var marketData = new MarketData
                                {
                                    Initialized = true,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = orderBook.Result
                                };

                                this._bitstamp.TryAdd(marketKey, marketData);

                                var sizeLimitedMarketData = LimitSize("0", marketData);
                                this._bitstamp[marketKey] = sizeLimitedMarketData;
                            }
                            else
                            {
                                this._bitstamp.TryAdd(marketKey, new MarketData
                                {
                                    Initialized = false,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = new OrderBookViewModel
                                    {
                                        asks = new List<OrderPriceAndQuantityViewModel>(),
                                        bids = new List<OrderPriceAndQuantityViewModel>()
                                    }
                                });
                            }



                        }
                    });
                }
                else if (exchangeId == "4")
                {
                    await Task.Run(() =>
                    {
                        lock (this._poloniex)
                        {
                            /*var orderBook = _exchangeApiClient.GetOrderBook(exchangeId, baseCurrency, quoteCurrency).Result;
                            var exist = Poloniex.Where(m => m.BaseCurrency == baseCurrency.ToUpper() && m.QuoteCurrency == quoteCurrency.ToUpper()).SingleOrDefault();
                            if (exist != null)
                                this.Poloniex.Remove(exist);

                            if (orderBook.ErrorMessage == null && orderBook.Result != null)
                            {

                                var marketData = new MarketData
                                {
                                    Initialized = false, //wait for event update
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = orderBook.Result
                                };

                                this.Poloniex.Add(marketData);
                                LimitSize("4", Poloniex.Where(m => m == marketData).SingleOrDefault());
                            }
                            else
                            {

                            }*/

                            //wait for event update
                            this._poloniex.TryAdd(
                                (baseCurrency + quoteCurrency).ToUpper(),
                                new MarketData
                                {
                                    Initialized = false,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = new OrderBookViewModel
                                    {
                                        asks = new List<OrderPriceAndQuantityViewModel>(),
                                        bids = new List<OrderPriceAndQuantityViewModel>()
                                    }
                                });

                        }
                    });
                }
                else if (exchangeId == "5")
                {
                    await Task.Run(() =>
                    {
                        lock (this.sync)
                        {
                            var marketKey = (baseCurrency + quoteCurrency).ToUpper();
                            var orderBook = _exchangeApiClient.GetOrderBook(exchangeId, baseCurrency, quoteCurrency).Result;
                            MarketData exist = null;

                            if (this._kucoin.TryGetValue(marketKey, out exist))
                            {
                                this._kucoin.Remove(marketKey);
                            }

                            if (orderBook.ErrorMessage == null && orderBook.Result != null)
                            {

                                var marketData = new MarketData
                                {
                                    Initialized = true,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = orderBook.Result
                                };

                                this._kucoin.TryAdd(marketKey, marketData);

                                var sizeLimitedMarketData = LimitSize("0", marketData);
                                this._kucoin[marketKey] = sizeLimitedMarketData;
                            }
                            else
                            {
                                this._kucoin.TryAdd(marketKey, new MarketData
                                {
                                    Initialized = false,
                                    BaseCurrency = baseCurrency.ToUpper(),
                                    QuoteCurrency = quoteCurrency.ToUpper(),
                                    OrderBook = new OrderBookViewModel
                                    {
                                        asks = new List<OrderPriceAndQuantityViewModel>(),
                                        bids = new List<OrderPriceAndQuantityViewModel>()
                                    }
                                });
                            }


                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Initialize exchange market failed." +
                    "Exchange Id: " + exchangeId +
                    "Error Message: " + ex.Message);
            }

        }


        public async Task WriteMarketData(string exchangeId, string baseCurrency, string quoteCurrency, string data)
        {
            await Task.Run(() =>
            {
                if (exchangeId == "0")
                {
                    string actionType = string.Empty;
                    JObject jsonResult = JObject.Parse(data);
                    baseCurrency = jsonResult["M"].ToString().Substring(5).ToUpper();
                    quoteCurrency = jsonResult["M"].ToString().Substring(0, 4).ToUpper();

                    var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                    MarketData marketData = null;
                    if (!this._bittrex.TryGetValue(marketKey, out marketData))
                    {
                        throw new KeyNotFoundException($"Invalid bittrex market: \"{baseCurrency}\"-\"{quoteCurrency}\".");
                    }
                    else if (marketData.Initialized == false)
                    {
                        throw new Exception($"Bittrex market \"{baseCurrency}\"-\"{quoteCurrency}\" isn't initialized.");
                    }

                    lock (this._bittrex[marketKey])
                    {
                        //Write buy orders.
                        foreach (var order in jsonResult["Z"])
                        {
                            actionType = order["TY"].ToString();



                            if (actionType == "0")
                            {
                                marketData.OrderBook.bids.Add(new OrderPriceAndQuantityViewModel
                                {
                                    price = Decimal.Parse(order["R"].ToString(), NumberStyles.Float),
                                    quantity = Decimal.Parse(order["Q"].ToString(), NumberStyles.Float)
                                });
                            }
                            else if (actionType == "1")
                            {
                                try
                                {
                                    var orderToRemove = marketData.OrderBook.bids.Where(o => o.price == Decimal.Parse(order["R"].ToString(), NumberStyles.Float)).SingleOrDefault();
                                    if (orderToRemove == null)
                                        throw new KeyNotFoundException($"Could not found order with price {order["R"]} on  Bittrex market for {baseCurrency + quoteCurrency} trading pair.");
                                    var success = marketData.OrderBook.bids.Remove(orderToRemove);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                            else if (actionType == "2")
                            {
                                try
                                {
                                    var orderToUpdate = marketData.OrderBook.bids.Where(o => o.price == Decimal.Parse(order["R"].ToString(), NumberStyles.Float)).SingleOrDefault();
                                    if (orderToUpdate == null)
                                        throw new KeyNotFoundException($"Could not found order with price {order["R"]} on  Bittrex market for {baseCurrency + quoteCurrency} trading pair.");
                                    orderToUpdate.quantity = Decimal.Parse(order["Q"].ToString(), System.Globalization.NumberStyles.Float);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }

                            }
                        }

                        //Write sell orders.
                        foreach (var order in jsonResult["S"])
                        {
                            actionType = order["TY"].ToString();
                            if (actionType == "0")
                            {
                                marketData.OrderBook.asks.Add(new OrderPriceAndQuantityViewModel
                                {
                                    price = Decimal.Parse(order["R"].ToString(), NumberStyles.Float),
                                    quantity = Decimal.Parse(order["Q"].ToString(), NumberStyles.Float)
                                });
                            }
                            else if (actionType == "1")
                            {
                                try
                                {
                                    var orderToRemove = marketData.OrderBook.asks.Where(o => o.price == Decimal.Parse(order["R"].ToString(), NumberStyles.Float)).SingleOrDefault();
                                    if (orderToRemove == null)
                                        throw new KeyNotFoundException($"Could not found order with price {order["R"]} on  Bittrex market for {baseCurrency + quoteCurrency} trading pair.");
                                    var success = marketData.OrderBook.asks.Remove(orderToRemove);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }

                            }
                            else if (actionType == "2")
                            {
                                //var orderToUpdate = marketData.OrderBook.bids.Where(o => o.price == order["R"].ToString());
                                try
                                {
                                    var orderToUpdate = marketData.OrderBook.asks.Where(o => o.price == Decimal.Parse(order["R"].ToString(), NumberStyles.Float)).SingleOrDefault();
                                    if (orderToUpdate == null)
                                        throw new KeyNotFoundException($"Could not found order with price {order["R"]} on  Bittrex market for {baseCurrency + quoteCurrency} trading pair.");
                                    orderToUpdate.quantity = Decimal.Parse(order["Q"].ToString(), NumberStyles.Float);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }


                        //Sort by price.
                        marketData.OrderBook.bids = marketData.OrderBook.bids.OrderByDescending(o => o.price, new PriceComparaer()).ToList();
                        marketData.OrderBook.asks = marketData.OrderBook.asks.OrderBy(o => o.price, new PriceComparaer()).ToList();


                        //Limit size.
                        var sizeLimitedMarketData = LimitSize("0", marketData);

                        //Update market.
                        this._bittrex[marketKey] = sizeLimitedMarketData;

                    }

                }
                else if (exchangeId == "1")
                {
                    var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                    MarketData marketData = null;
                    if (!this._binance.TryGetValue(marketKey, out marketData))
                    {
                        throw new KeyNotFoundException($"Invalid binance market: \"{baseCurrency}\"-\"{quoteCurrency}\".");
                    }
                    else if (marketData.Initialized == false)
                    {
                        throw new Exception($"Binance market \"{baseCurrency}\"-\"{quoteCurrency}\" isn't initialized.");
                    }

                    lock (this._binance[marketKey])
                    {
                        JObject jsonResult = JObject.Parse(data);
                        //string baseCurrency = jsonResult["Symbol"].ToString().Substring(0, 3).ToUpper();
                        //string quoteCurrency = jsonResult["Symbol"].ToString().Substring(3).ToUpper();

                        var bidsJson = jsonResult["Bids"];
                        var asksJson = jsonResult["Asks"];

                        var asksList = new List<OrderPriceAndQuantityViewModel>();
                        var bidsList = new List<OrderPriceAndQuantityViewModel>();

                        foreach (var order in asksJson)
                        {
                            var quantity = Decimal.Parse(order[1].ToString(), System.Globalization.NumberStyles.Float);
                            asksList.Add(
                                 new OrderPriceAndQuantityViewModel
                                 {
                                     price = Decimal.Parse(order[0].ToString()),
                                     quantity = quantity
                                 }
                                 );
                        }
                        foreach (var order in bidsJson)
                        {
                            var quantity = Decimal.Parse(order[1].ToString(), System.Globalization.NumberStyles.Float);
                            bidsList.Add(
                                 new OrderPriceAndQuantityViewModel
                                 {
                                     price = Decimal.Parse(order[0].ToString()),
                                     quantity = quantity
                                 }
                                 );
                        }
                        marketData.OrderBook.asks = asksList;
                        marketData.OrderBook.bids = bidsList;

                        this._binance[marketKey] = marketData;
                    }

                }
                else if (exchangeId == "3")
                {
                    var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                    MarketData marketData = null;
                    if (!this._bitstamp.TryGetValue(marketKey, out marketData))
                    {
                        throw new KeyNotFoundException($"Invalid bitstamp market: \"{baseCurrency}\"-\"{quoteCurrency}\".");
                    }
                    else if (marketData.Initialized == false)
                    {
                        //throw new Exception($"Bitstamp market \"{baseCurrency}\"-\"{quoteCurrency}\" isn't initialized.");
                    }

                    lock (this._bitstamp)
                    {
                        JObject jsonResult = JObject.Parse(data);
                        //string baseCurrency = jsonResult["market"].ToString().Substring(0, 3).ToUpper();
                        //string quoteCurrency = jsonResult["market"].ToString().Substring(3).ToUpper();

                        var bidsJson = jsonResult["bids"];
                        var asksJson = jsonResult["asks"];

                        var asksList = new List<OrderPriceAndQuantityViewModel>();
                        var bidsList = new List<OrderPriceAndQuantityViewModel>();
                        foreach (var order in asksJson)
                        {
                            var quantity = Decimal.Parse(order[1].ToString(), System.Globalization.NumberStyles.Float);
                            asksList.Add(
                                 new OrderPriceAndQuantityViewModel
                                 {
                                     price = Decimal.Parse(order[0].ToString()),
                                     quantity = quantity
                                 }
                                 );
                        }
                        foreach (var order in bidsJson)
                        {
                            var quantity = Decimal.Parse(order[1].ToString(), System.Globalization.NumberStyles.Float);
                            bidsList.Add(
                                 new OrderPriceAndQuantityViewModel
                                 {
                                     price = Decimal.Parse(order[0].ToString()),
                                     quantity = quantity
                                 }
                                 );
                        }

                        marketData.OrderBook.asks = asksList;
                        marketData.OrderBook.bids = bidsList;

                        //Sort by price.
                        /*marketData.OrderBook.bids = marketData.OrderBook.bids.OrderByDescending(o => o.price, new PriceComparaer()).ToList();
                        marketData.OrderBook.asks = marketData.OrderBook.asks.OrderBy(o => o.price, new PriceComparaer()).ToList();*/

                        //Limit order's total amount.
                        var sizeLimitedMarketData = LimitSize("3", marketData);

                        this._bitstamp[marketKey] = sizeLimitedMarketData;
                    }
                }
                else if (exchangeId == "4")
                {
                    var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                    MarketData marketData = null;
                    if (!this._poloniex.TryGetValue(marketKey, out marketData))
                    {
                        throw new KeyNotFoundException($"Invalid poloniex market: \"{baseCurrency}\"-\"{quoteCurrency}\".");
                    }

                    lock (this._poloniex)
                    {
                        JObject jsonResult = JObject.Parse("{\"result\":" + data + "}");

                        //var market = jsonResult["result"][0].ToString();
                        //string baseCurrency = market.Substring(0, 3);
                        //string quoteCurrency = market.Substring(3);

                        if (marketData.Initialized == false)
                        {
                            //throw new Exception($"Poloniex market \"{baseCurrency}\"-\"{quoteCurrency}\" isn't initialized.");
                            var bidsJson = JObject.Parse(jsonResult["result"][2][0][1]["orderBook"][1].ToString());
                            var asksJson = JObject.Parse(jsonResult["result"][2][0][1]["orderBook"][0].ToString());


                            var asksList = new List<OrderPriceAndQuantityViewModel>();
                            var bidsList = new List<OrderPriceAndQuantityViewModel>();

                            foreach (var order in asksJson)
                            {

                                asksList.Add(
                                     new OrderPriceAndQuantityViewModel
                                     {
                                         price = Decimal.Parse(order.Key),
                                         quantity = Decimal.Parse(order.Value.ToString())
                                     }
                                     );
                            }
                            foreach (var order in bidsJson)
                            {
                                bidsList.Add(
                                     new OrderPriceAndQuantityViewModel
                                     {
                                         price = Decimal.Parse(order.Key),
                                         quantity = Decimal.Parse(order.Value.ToString())
                                     }
                                     );
                            }
                            marketData.OrderBook.asks = asksList;
                            marketData.OrderBook.bids = bidsList;

                            marketData.Initialized = true;
                            this._poloniex[marketKey] = marketData;
                        }
                        else
                        {
                            //Write orders.
                            foreach (var order in jsonResult["result"][2])
                            {
                                string actionType = string.Empty;
                                actionType = order[0].ToString();


                                if (actionType == "o")  //new order
                                {
                                    //buy
                                    if (order[1].ToString() == "1")
                                    {
                                        var price = Decimal.Parse(order[2].ToString());
                                        var quantity = Decimal.Parse(order[3].ToString());
                                        var orderToUpdate = marketData.OrderBook.bids.Where(o => o.price == price).SingleOrDefault();
                                        if (orderToUpdate == null)
                                        {
                                            if (quantity > 0)
                                            {
                                                marketData.OrderBook.bids.Add(new OrderPriceAndQuantityViewModel
                                                {
                                                    price = Decimal.Parse(order[2].ToString()),
                                                    quantity = Decimal.Parse(order[3].ToString())
                                                });
                                            }

                                        }
                                        else
                                        {
                                            if (quantity > 0)
                                            {
                                                orderToUpdate.quantity = quantity;
                                            }
                                            else
                                            {
                                                var success = marketData.OrderBook.bids.Remove(orderToUpdate);
                                            }
                                        }
                                    }

                                    //sell
                                    if (order[1].ToString() == "0")
                                    {
                                        var price = Decimal.Parse(order[2].ToString());
                                        var quantity = Decimal.Parse(order[3].ToString());
                                        var orderToUpdate = marketData.OrderBook.asks.Where(o => o.price == price).SingleOrDefault();
                                        if (orderToUpdate == null)
                                        {
                                            if (quantity > 0)
                                            {
                                                marketData.OrderBook.asks.Add(new OrderPriceAndQuantityViewModel
                                                {
                                                    price = Decimal.Parse(order[2].ToString()),
                                                    quantity = Decimal.Parse(order[3].ToString())
                                                });
                                            }
                                        }
                                        else
                                        {
                                            if (quantity > 0)
                                            {
                                                orderToUpdate.quantity = quantity;
                                            }
                                            else
                                            {
                                                var success = marketData.OrderBook.asks.Remove(orderToUpdate);
                                            }
                                        }
                                    }
                                }

                            }
                        }


                        //Sort by price.
                        marketData.OrderBook.bids = marketData.OrderBook.bids.OrderByDescending(o => o.price, new PriceComparaer()).ToList();
                        marketData.OrderBook.asks = marketData.OrderBook.asks.OrderBy(o => o.price, new PriceComparaer()).ToList();

                        var sizeLimitedMarketData = LimitSize("4", marketData);

                        this._poloniex[marketKey] = sizeLimitedMarketData;
                    }
                }
                else if (exchangeId == "5")
                {
                    var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                    MarketData marketData = null;
                    if (!this._kucoin.TryGetValue(marketKey, out marketData))
                    {
                        throw new KeyNotFoundException($"Invalid kucoin market: \"{baseCurrency}\"-\"{quoteCurrency}\".");
                    }

                    lock (this._kucoin[marketKey])
                    {
                        JObject jsonResult = JObject.Parse(data);
                        //string baseCurrency = jsonResult["Symbol"].ToString().Substring(0, 3).ToUpper();
                        //string quoteCurrency = jsonResult["Symbol"].ToString().Substring(3).ToUpper();

                        var order = jsonResult["data"];


                        string actionType = string.Empty;
                        actionType = order["action"].ToString();


                        if (actionType == "ADD")  //new order
                        {
                            //buy
                            if (order["type"].ToString() == "BUY")
                            {
                                var price = Decimal.Parse(order["price"].ToString(), System.Globalization.NumberStyles.Float);
                                var quantity = Decimal.Parse(order["count"].ToString(), System.Globalization.NumberStyles.Float);
                                var orderToUpdate = marketData.OrderBook.bids.Where(o => o.price == price).SingleOrDefault();
                                if (orderToUpdate == null)
                                {
                                    if (quantity > 0)
                                    {
                                        marketData.OrderBook.bids.Add(new OrderPriceAndQuantityViewModel
                                        {
                                            price = price,
                                            quantity = quantity
                                        });
                                    }

                                }
                                else
                                {
                                    if (quantity > 0)
                                    {
                                        orderToUpdate.quantity = quantity;
                                    }
                                    else
                                    {
                                        var success = marketData.OrderBook.bids.Remove(orderToUpdate);
                                    }
                                }
                            }

                            //sell
                            if (order["type"].ToString() == "SELL")
                            {
                                var price = Decimal.Parse(order["price"].ToString(), NumberStyles.Float);
                                var quantity = Decimal.Parse(order["count"].ToString(), NumberStyles.Float);
                                var orderToUpdate = marketData.OrderBook.asks.Where(o => o.price == price).SingleOrDefault();
                                if (orderToUpdate == null)
                                {
                                    if (quantity > 0)
                                    {
                                        marketData.OrderBook.asks.Add(new OrderPriceAndQuantityViewModel
                                        {
                                            price = price,
                                            quantity = quantity
                                        });
                                    }
                                }
                                else
                                {
                                    if (quantity > 0)
                                    {
                                        orderToUpdate.quantity = quantity;
                                    }
                                    else
                                    {
                                        var success = marketData.OrderBook.asks.Remove(orderToUpdate);
                                    }
                                }
                            }
                        }
                        else if (actionType == "CANCEL")
                        {
                            if (order["type"].ToString() == "BUY")
                            {
                                var price = Decimal.Parse(order["price"].ToString(), NumberStyles.Float);

                                var orderToDelete = marketData.OrderBook.bids.Where(o => o.price == price).SingleOrDefault();
                                if (orderToDelete != null)
                                {
                                    var success = marketData.OrderBook.bids.Remove(orderToDelete);
                                }
                            }
                            else if (order["type"].ToString() == "SELL")
                            {
                                var price = Decimal.Parse(order["price"].ToString(), NumberStyles.Float);

                                var orderToDelete = marketData.OrderBook.asks.Where(o => o.price == price).SingleOrDefault();
                                if (orderToDelete != null)
                                {
                                    var success = marketData.OrderBook.asks.Remove(orderToDelete);
                                }
                            }
                        }


                        //Sort by price.
                        marketData.OrderBook.bids = marketData.OrderBook.bids.OrderByDescending(o => o.price, new PriceComparaer()).ToList();
                        marketData.OrderBook.asks = marketData.OrderBook.asks.OrderBy(o => o.price, new PriceComparaer()).ToList();

                        var sizeLimitedMarketData = LimitSize("5", marketData);

                        this._kucoin[marketKey] = sizeLimitedMarketData;
                    }


                }
            });
        }


        public OrderBookViewModel GetMarketData(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            var orderBook = new OrderBookViewModel();
            if (exchangeId == "0")
            {
                var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                MarketData marketData = null;



                if (!this._bittrex.TryGetValue(marketKey, out marketData))
                    throw new NullReferenceException($"Bittrex market data \"{baseCurrency}-{quoteCurrency}\" is still initializing.");

                orderBook.asks = marketData.OrderBook.asks.ToList();
                orderBook.bids = marketData.OrderBook.bids.ToList();

                return orderBook;
            }
            else if (exchangeId == "1")
            {
                var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                MarketData marketData = null;

                if (!this._binance.TryGetValue(marketKey, out marketData))
                    throw new NullReferenceException($"Binance market data \"{baseCurrency}-{quoteCurrency}\" is still initializing.");

                orderBook.asks = marketData.OrderBook.asks.ToList();
                orderBook.bids = marketData.OrderBook.bids.ToList();

                return orderBook;
            }
            else if (exchangeId == "3")
            {
                var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                MarketData marketData = null;

                if (!this._bitstamp.TryGetValue(marketKey, out marketData))
                    throw new NullReferenceException($"Bitstamp market data \"{baseCurrency}-{quoteCurrency}\" is still initializing.");

                orderBook.asks = marketData.OrderBook.asks.ToList();
                orderBook.bids = marketData.OrderBook.bids.ToList();

                return orderBook;
            }
            else if (exchangeId == "4")
            {
                var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                MarketData marketData = null;

                if (!this._poloniex.TryGetValue(marketKey, out marketData))
                    throw new NullReferenceException($"Poloniex market data \"{baseCurrency}-{quoteCurrency}\" is still initializing.");

                orderBook.asks = marketData.OrderBook.asks.ToList();
                orderBook.bids = marketData.OrderBook.bids.ToList();

                return orderBook;
            }
            else if (exchangeId == "5")
            {
                var marketKey = (baseCurrency + quoteCurrency).ToUpper();

                MarketData marketData = null;



                if (!this._kucoin.TryGetValue(marketKey, out marketData))
                    throw new NullReferenceException($"Kucoin market data \"{baseCurrency}-{quoteCurrency}\" is still initializing.");

                orderBook.asks = marketData.OrderBook.asks.ToList();
                orderBook.bids = marketData.OrderBook.bids.ToList();

                return orderBook;
            }
            throw new KeyNotFoundException("Exchange Not Found");
        }


        private MarketData LimitSize(string exchangeId, MarketData marketData)
        {
            int count = 0;
            bool shouldUpdate = false;

            count = marketData.OrderBook.asks.Count;
            if (count > 50)
            {
                marketData.OrderBook.asks.RemoveRange(50, count - 50);
                shouldUpdate = true;
            }

            count = marketData.OrderBook.bids.Count;
            if (count > 50)
            {
                marketData.OrderBook.bids.RemoveRange(50, count - 50);
                shouldUpdate = true;
            }

            return marketData;

            /*if (exchangeId == "0")
            {
                if (shouldUpdate)
                {
                    var oldMarketData = this._bittrex.Where(m => m.BaseCurrency == marketData.BaseCurrency && m.QuoteCurrency == marketData.QuoteCurrency)
                                                     .SingleOrDefault();
                    oldMarketData = marketData;
                }
            }
            else if (exchangeId == "1")
            {
                if (shouldUpdate)
                {
                    var oldMarketData = this._binance.Where(m => m.BaseCurrency == marketData.BaseCurrency && m.QuoteCurrency == marketData.QuoteCurrency)
                                                     .SingleOrDefault();
                    oldMarketData = marketData;
                }
            }
            else if (exchangeId == "3")
            {
                if (shouldUpdate)
                {
                    var oldMarketData = this._bitstamp.Where(m => m.BaseCurrency == marketData.BaseCurrency && m.QuoteCurrency == marketData.QuoteCurrency)
                                                     .SingleOrDefault();
                    oldMarketData = marketData;
                }
            }
            else if (exchangeId == "4")
            {
                if (shouldUpdate)
                {
                    var oldMarketData = this._poloniex.Where(m => m.BaseCurrency == marketData.BaseCurrency && m.QuoteCurrency == marketData.QuoteCurrency)
                                                     .SingleOrDefault();
                    oldMarketData = marketData;
                }
            }*/
        }

        class PriceComparaer : IComparer<decimal>
        {
            public int Compare(decimal x, decimal y)
            {
                return x.CompareTo(y);
            }
        }

        public class MarketData
        {
            public bool Initialized { get; set; }
            public string BaseCurrency { get; set; }
            public string QuoteCurrency { get; set; }
            public OrderBookViewModel OrderBook { get; set; }
        }
    }
}
