using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Utilitys;
using ExchangeSharp;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using gateio.api;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public class ExchangeApiClient : IExchangeApiClient
    {
        private readonly IOptions<ExchangeApiSetting> _exchangeApiSetting;

        public ExchangeApiClient(IOptions<ExchangeApiSetting> exchagneApiSetting)
        {
            _exchangeApiSetting = exchagneApiSetting;
            gateio.api.API.SetKey(
                           this._exchangeApiSetting.Value.Gateio.ApiKey,
                           this._exchangeApiSetting.Value.Gateio.ApiSecret);
        }

        public async Task<ExchangeApiResponse<OrderBookViewModel>> GetOrderBook(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            try
            {
                var apiMethod = ExchangeApiMethodEnum.GetMarket;
                string uri = string.Empty;
                HttpMethod httpMethod = HttpMethod.Get;

                baseCurrency = baseCurrency.ToUpper();
                quoteCurrency = quoteCurrency.ToUpper();

                if (exchangeId == "0")
                {
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        quoteCurrency = "USDT";
                    }
                    uri = "https://bittrex.com/api/v1.1/public/getorderbook?market=" + quoteCurrency + "-" + baseCurrency + "&type=both";


                    return await this.Call<OrderBookViewModel>(
                      exchangeId,
                      uri,
                      httpMethod,
                      apiMethod
                      );

                }
                else if (exchangeId == "1")
                {
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        quoteCurrency = "USDT";
                    }
                    uri = "https://api.binance.com/api/v1/depth?symbol=" + baseCurrency + quoteCurrency;
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage res = await client.GetAsync(uri))
                        {
                            using (HttpContent content = res.Content)
                            {
                                var orderBook = await ExchangeResponseWrapper.ParseResponse(exchangeId.ToString(), ExchangeApiMethodEnum.GetMarket, res);

                                if (orderBook != null)
                                {
                                    return new ExchangeApiResponse<OrderBookViewModel>
                                    {
                                        ErrorMessage = orderBook.ErrorMessage,
                                        StatusCode = orderBook.StatusCode,
                                        Result = orderBook.Result as OrderBookViewModel,
                                        Timestamp = orderBook.Timestamp
                                    };

                                }
                            }
                        }
                    }
                }
                else if (exchangeId == "2")
                {
                    httpMethod = HttpMethod.Post;

                    if (quoteCurrency.ToUpper() == "BTC")
                    {
                        quoteCurrency = "XBT";
                    }
                    if (baseCurrency.ToUpper() == "BTC")
                    {
                        baseCurrency = "XBT";
                    }
                    var market = (baseCurrency + quoteCurrency).ToUpper();
                    var limitSize = "20";
                    uri = this.GetBaseUrl(exchangeId, apiMethod) + "?" + "pair=" + market + "&count=" + limitSize;

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage res = await client.GetAsync(uri))
                        {
                            using (HttpContent content = res.Content)
                            {
                                var orderBook = await ExchangeResponseWrapper.ParseResponse(exchangeId.ToString(), ExchangeApiMethodEnum.GetMarket, res);

                                if (orderBook != null)
                                {
                                    return new ExchangeApiResponse<OrderBookViewModel>
                                    {
                                        ErrorMessage = orderBook.ErrorMessage,
                                        StatusCode = orderBook.StatusCode,
                                        Result = orderBook.Result as OrderBookViewModel,
                                        Timestamp = orderBook.Timestamp
                                    };

                                }
                            }
                        }
                    }
                }
                else if (exchangeId == "3")
                {
                    var market = (baseCurrency + quoteCurrency).ToLower();
                    uri = "https://www.bitstamp.net/api/v2/order_book/" + market;


                    return await this.Call<OrderBookViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod
                        );
                }
                else if (exchangeId == "4")
                {
                    var market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangePoloniexAPI())

                    {
                        ExchangeOrderBook orderBook = await client.GetOrderBookAsync(market, 50);
                        return new ExchangeApiResponse<OrderBookViewModel>
                        {
                            ErrorMessage = null,
                            Result = orderBook.ToOrderBookViewModel(),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                else if(exchangeId=="5")
                {
                    var market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangeKucoinAPI())

                    {
                        ExchangeOrderBook orderBook = await client.GetOrderBookAsync(market, 50);
                        return new ExchangeApiResponse<OrderBookViewModel>
                        {
                            ErrorMessage = null,
                            Result = orderBook.ToOrderBookViewModel(),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                /*else if (exchangeId == "5")
                {
                    var market = (baseCurrency + quoteCurrency).ToLower();
                    var orderBook = await gateio.api.API.OrderBookAsync(baseCurrency.ToLower(), quoteCurrency.ToLower());
                    return new ExchangeApiResponse<OrderBookViewModel>
                    {
                        ErrorMessage = null,
                        Result = orderBook.ToOrderBookViewModel(),
                        StatusCode = HttpStatusCode.OK,
                        Timestamp = DateTime.Now
                    };
                }*/
            }
            catch (Exception ex)
            {
                /*return new ExchangeApiResponse<OrderBookViewModel>
                {
                    ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                };*/
                return new ExchangeApiResponse<OrderBookViewModel>
                {
                    ErrorMessage = String.Format(ex.Message),
                    StatusCode = HttpStatusCode.BadRequest,
                    Timestamp = DateTime.Now
                };
            }

            return new ExchangeApiResponse<OrderBookViewModel>
            {
                ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
            };

        }


        public async Task<ExchangeApiResponse<GetOpenOrderViewModel>> GetOpenOrder(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            try
            {
                ExchangeAPI exchangeAPI = null;
                string uri = this.GetBaseUrl(exchangeId, ExchangeApiMethodEnum.GetOpenOrders);
                //string hashFunction = null;
                var nonce = DateTime.Now.Ticks.ToString();
                var timestamp = (long)DateTime.Now.Millisecond;
                var apiMethod = ExchangeApiMethodEnum.GetOpenOrders;

                if (exchangeId == "0")
                {

                    string apiKey = _exchangeApiSetting.Value.Bittrex.ApiKey;
                    string apiSecret = _exchangeApiSetting.Value.Bittrex.ApiSecret;

                    string baseUrl = "https://bittrex.com/api/v1.1/market/getopenorders";

                    exchangeAPI = _exchangeApiSetting.Value.Bittrex;
                    uri = string.Format(baseUrl + "?apikey=" + apiKey + "&nonce=" + nonce);
                    //hashFunction = ExchangeDelegatingHandler.HASH_HMAC512;

                    return await this.Call<GetOpenOrderViewModel>(
                        exchangeId,
                        uri,
                        HttpMethod.Post,
                        apiMethod
                        );
                }
                else if (exchangeId == "1")
                {
                    if (!String.IsNullOrEmpty(quoteCurrency))
                    {
                        if (quoteCurrency.ToUpper() == "USD")
                        {
                            quoteCurrency = "USDT";
                        }
                    }

                    string baseUrl = "https://api.binance.com/api/v3/openOrders";


                    exchangeAPI = _exchangeApiSetting.Value.Binance;
                    //uri = string.Format(baseUrl + "?timestamp=" + timestamp+"&recvWindow="+"10000000");
                    uri = string.Format(baseUrl);

                    //hashFunction = ExchangeDelegatingHandler.HASH_HMAC256;
                    return await this.Call<GetOpenOrderViewModel>(
                       exchangeId,
                       uri,
                       HttpMethod.Get,
                       apiMethod
                       );
                }
                else if (exchangeId == "2")
                {
                    if (!String.IsNullOrEmpty(quoteCurrency) && !String.IsNullOrEmpty(baseCurrency))
                    {
                        if (quoteCurrency.ToUpper() == "BTC")
                        {
                            quoteCurrency = "XBT";
                        }
                        if (baseCurrency.ToUpper() == "BTC")
                        {
                            baseCurrency = "XBT";
                        }
                    }
                    return await this.Call<GetOpenOrderViewModel>(
                         exchangeId,
                         uri,
                         HttpMethod.Post,
                         apiMethod
                         );
                }
                else if (exchangeId == "3")
                {
                    exchangeAPI = _exchangeApiSetting.Value.Bitstamp;
                    var market = (baseCurrency + quoteCurrency).ToLower();
                    return await this.Call<GetOpenOrderViewModel>(
                        exchangeId,
                        uri + "/" + "?" + market,
                        HttpMethod.Post,
                        apiMethod
                        );
                }
                else if (exchangeId == "4")
                {
                    var market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangePoloniexAPI())

                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Poloniex.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Poloniex.ApiSecret.ToSecureString();

                        var result = await client.GetOpenOrderDetailsAsync(market);
                        List<ExchangeOrderResult> openOrders = null;

                        if (result != null)
                            openOrders = result.ToList();

                        return new ExchangeApiResponse<GetOpenOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = openOrders.ToGetOpenOrderViewModel(exchangeId),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                else if (exchangeId == "5")
                {
                    var market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangeKucoinAPI())

                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Kucoin.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Kucoin.ApiSecret.ToSecureString();

                        var result = await client.GetOpenOrderDetailsAsync(market);
                        List<ExchangeOrderResult> openOrders = null;

                        if (result != null)
                            openOrders = result.ToList();

                        return new ExchangeApiResponse<GetOpenOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = openOrders.ToGetOpenOrderViewModel(exchangeId),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                /*else if (exchangeId == "5")
                {
                    return await this.Call<GetOpenOrderViewModel>(
                         exchangeId,
                         uri,
                         HttpMethod.Post,
                         apiMethod
                         );
                }*/

            }
            catch (Exception ex)
            {
                /*return new ExchangeApiResponse<OrderBookViewModel>
                {
                    ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                };*/
                return new ExchangeApiResponse<GetOpenOrderViewModel>
                {
                    ErrorMessage = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest,
                    Timestamp = DateTime.Now
                };
            }


            return new ExchangeApiResponse<GetOpenOrderViewModel>
            {
                ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                StatusCode = HttpStatusCode.BadRequest,
                Timestamp = DateTime.Now
            };

        }


        public async Task<ExchangeApiResponse<BalanceViewModel>> GetBalances(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            try
            {
                var apiMethod = ExchangeApiMethodEnum.GetBalances;
                var uri = this.GetBaseUrl(exchangeId.ToString(), apiMethod);

                if (exchangeId == "0")
                {

                    string market = !(string.IsNullOrEmpty(baseCurrency) || string.IsNullOrEmpty(quoteCurrency)) ? quoteCurrency + "-" + baseCurrency : string.Empty;

                    return await this.Call<BalanceViewModel>(
                        exchangeId.ToString(),
                        uri,
                        HttpMethod.Get,
                        apiMethod,
                        Tuple.Create("currency", market)
                   );
                }
                else if (exchangeId == "1")
                {
                    //string market = !(string.IsNullOrEmpty(baseCurrency) || string.IsNullOrEmpty(quoteCurrency)) ? baseCurrency + quoteCurrency : string.Empty;
                    /*if (!String.IsNullOrEmpty(quoteCurrency))
                    {
                        if (quoteCurrency.ToUpper() == "USD")
                        {
                            quoteCurrency = "USDT";
                        }
                    }*/
                    return await this.Call<BalanceViewModel>(
                        exchangeId.ToString(),
                        uri,
                        HttpMethod.Get,
                        apiMethod
                   );
                }
                if (exchangeId == "2")
                {

                    //string market = !(string.IsNullOrEmpty(baseCurrency) || string.IsNullOrEmpty(quoteCurrency)) ? quoteCurrency + "-" + baseCurrency : string.Empty;

                    return await this.Call<BalanceViewModel>(
                        exchangeId.ToString(),
                        uri,
                        HttpMethod.Post,
                        apiMethod
                   );
                }
                else if (exchangeId == "3")
                {
                    string market = !(string.IsNullOrEmpty(baseCurrency) || string.IsNullOrEmpty(quoteCurrency)) ? (baseCurrency + quoteCurrency + "/").ToLower() : string.Empty;
                    return await this.Call<BalanceViewModel>(
                        exchangeId.ToString(),
                        uri + market,
                        HttpMethod.Post,
                        apiMethod
                        );
                }
                else if (exchangeId == "4")
                {
                    //var market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangePoloniexAPI())

                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Poloniex.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Poloniex.ApiSecret.ToSecureString();

                        var balancesResult = await client.GetAmountsAsync();
                        var availableBalancesResult = await client.GetAmountsAvailableToTradeAsync();

                        BalanceViewModel balances;
                        ExchangeSharpParser.TryGetBalanceViewModel(out balances, balancesResult, availableBalancesResult);

                        return new ExchangeApiResponse<BalanceViewModel>
                        {
                            ErrorMessage = null,
                            Result = balances == null ? null : balances,
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                else if (exchangeId == "5")
                {
                    //var market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangeKucoinAPI())

                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Kucoin.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Kucoin.ApiSecret.ToSecureString();

                        var balancesResult = await client.GetAmountsAsync();
                        var availableBalancesResult = await client.GetAmountsAvailableToTradeAsync();

                        BalanceViewModel balances;
                        ExchangeSharpParser.TryGetBalanceViewModel(out balances, balancesResult, availableBalancesResult);

                        return new ExchangeApiResponse<BalanceViewModel>
                        {
                            ErrorMessage = null,
                            Result = balances == null ? null : balances,
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }



                /*else if (exchangeId == "5")
                {
                  
                    gateio.api.API.SetKey(
                           this._exchangeApiSetting.Value.Gateio.ApiKey,
                           this._exchangeApiSetting.Value.Gateio.ApiSecret);

                    var balancesResult = await gateio.api.API.GetBalancesAsync();

                    return new ExchangeApiResponse<BalanceViewModel>
                    {
                        ErrorMessage = null,
                        Result = balancesResult == null ? null : balancesResult.ToBalanceViewModel(),
                        StatusCode = HttpStatusCode.OK,
                        Timestamp = DateTime.Now
                    };

                }*/

            }
            catch (Exception ex)
            {
                return new ExchangeApiResponse<BalanceViewModel>
                {
                    ErrorMessage = ex.Message,
                    Result = null,
                    StatusCode = HttpStatusCode.NotFound,
                    Timestamp = DateTime.Now
                };
            }

            return new ExchangeApiResponse<BalanceViewModel>
            {
                ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",

            };
        }


        public async Task<ExchangeApiResponse<PlaceBuyOrderViewModel>> PlaceBuyOrder(string exchangeId, string baseCurrency, string quoteCurrency, string quantity, string price)
        {
            try
            {
                var apiMethod = ExchangeApiMethodEnum.PlaceBuyOrder;
                var uri = this.GetBaseUrl(exchangeId, apiMethod);
                var httpMethod = HttpMethod.Post;

                string market = baseCurrency + quoteCurrency;

                if (exchangeId == "0")
                {
                    market = quoteCurrency + "-" + baseCurrency;
                    uri += "&";
                    return await this.Call<PlaceBuyOrderViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("market", market),
                        Tuple.Create("quantity", quantity),
                        Tuple.Create("rate", price)
                        );
                }
                else if (exchangeId == "1")
                {
                    uri += "&";
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        market = baseCurrency + "USDT";
                    }
                    market = market.ToUpper();
                    return await this.Call<PlaceBuyOrderViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("symbol", market),
                        Tuple.Create("quantity", quantity),
                        Tuple.Create("price", price)
                        );
                }
                else if (exchangeId == "2")
                {
                    uri += "&";

                    if (!String.IsNullOrEmpty(quoteCurrency) && !String.IsNullOrEmpty(baseCurrency))
                    {
                        if (quoteCurrency.ToUpper() == "BTC")
                        {
                            quoteCurrency = "XBT";
                        }
                        if (baseCurrency.ToUpper() == "BTC")
                        {
                            baseCurrency = "XBT";
                        }
                        market = baseCurrency + quoteCurrency;
                    }

                    market = market.ToUpper();

                    return await this.Call<PlaceBuyOrderViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("pair", market),
                        Tuple.Create("price", price),
                        Tuple.Create("volume", quantity)
                        );
                }
                else if (exchangeId == "3")
                {

                    return await this.Call<PlaceBuyOrderViewModel>(

                        exchangeId,
                        uri + market.ToLower() + "/" + "?",
                        httpMethod,
                        apiMethod,
                        Tuple.Create("amount", quantity),
                        Tuple.Create("price", price)
                        );
                }
                else if (exchangeId == "4")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangePoloniexAPI())

                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Poloniex.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Poloniex.ApiSecret.ToSecureString();


                        var result = await client.PlaceOrderAsync(new ExchangeOrderRequest
                        {
                            Amount = Decimal.Parse(quantity),
                            Price = Decimal.Parse(price),
                            Symbol = market,
                            OrderType = ExchangeSharp.OrderType.Limit,
                            IsBuy = true
                        });

                        return new ExchangeApiResponse<PlaceBuyOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = result.ToPlaceBuyOrderViewModel(),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                else if (exchangeId == "5")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);


                    using (var client = new ExchangeKucoinAPI())

                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Kucoin.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Kucoin.ApiSecret.ToSecureString();


                        var result = await client.PlaceOrderAsync(new ExchangeOrderRequest
                        {
                            Amount = Decimal.Parse(quantity),
                            Price = Decimal.Parse(price),
                            Symbol = market,
                            OrderType = ExchangeSharp.OrderType.Limit,
                            IsBuy = true
                        });

                        return new ExchangeApiResponse<PlaceBuyOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = result.ToPlaceBuyOrderViewModel(),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                /*else if (exchangeId == "5")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    gateio.api.API.SetKey(
                       this._exchangeApiSetting.Value.Gateio.ApiKey,
                       this._exchangeApiSetting.Value.Gateio.ApiSecret);

                    var orderId = await gateio.api.API.BuyAsync(new gateio.api.Model.OrderReq
                    {
                        CurrencyPair = market,
                        Amount = Decimal.Parse(quantity, System.Globalization.NumberStyles.Float),
                        Rate = Decimal.Parse(price, System.Globalization.NumberStyles.Float)
                    });

                    if (orderId != null)
                    {
                        return new ExchangeApiResponse<PlaceBuyOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = new PlaceBuyOrderViewModel { orderId = orderId },
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }*/


            }
            catch (Exception ex)
            {
                /*return new ExchangeApiResponse<OrderBookViewModel>
                {
                    ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                };*/
                return new ExchangeApiResponse<PlaceBuyOrderViewModel>
                {
                    ErrorMessage = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest,
                    Timestamp = DateTime.Now
                };
            }

            return new ExchangeApiResponse<PlaceBuyOrderViewModel>
            {
                ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                StatusCode = HttpStatusCode.BadRequest,
                Timestamp = DateTime.Now
            };
        }


        public async Task<ExchangeApiResponse<PlaceSellOrderViewModel>> PlaceSellOrder(string exchangeId, string baseCurrency, string quoteCurrency, string quantity, string price)
        {
            try
            {
                var apiMethod = ExchangeApiMethodEnum.PlaceSellOrder;
                var uri = this.GetBaseUrl(exchangeId, apiMethod);
                var httpMethod = HttpMethod.Post;
                string market = baseCurrency + quoteCurrency;

                if (exchangeId == "0")
                {
                    market = quoteCurrency + "-" + baseCurrency;
                    uri += "&";
                    return await this.Call<PlaceSellOrderViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("market", market), Tuple.Create("quantity", quantity), Tuple.Create("rate", price)
                        );
                }
                else if (exchangeId == "1")
                {
                    uri += "&";

                    if (!String.IsNullOrEmpty(quoteCurrency))
                    {
                        if (quoteCurrency.ToUpper() == "USD")
                        {
                            market = baseCurrency + "USDT";
                        }
                    }

                    market = market.ToUpper();
                    return await this.Call<PlaceSellOrderViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("symbol", market),
                        Tuple.Create("quantity", quantity),
                        Tuple.Create("price", price)
                   );
                }
                else if (exchangeId == "2")
                {
                    uri += "&";

                    if (!String.IsNullOrEmpty(quoteCurrency) && !String.IsNullOrEmpty(baseCurrency))
                    {
                        if (quoteCurrency.ToUpper() == "BTC")
                        {
                            quoteCurrency = "XBT";
                        }
                        if (baseCurrency.ToUpper() == "BTC")
                        {
                            baseCurrency = "XBT";
                        }
                        market = baseCurrency + quoteCurrency;
                    }

                    market = market.ToUpper();

                    return await this.Call<PlaceSellOrderViewModel>(
                        exchangeId,
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("pair", market),
                        Tuple.Create("price", price),
                        Tuple.Create("volume", quantity)
                        );
                }
                else if (exchangeId == "3")
                {

                    return await this.Call<PlaceSellOrderViewModel>(
                        exchangeId,
                        uri + market.ToLower() + "/" + "?",
                        httpMethod,
                        apiMethod,
                        Tuple.Create("amount", quantity),
                        Tuple.Create("price", price)
                        );
                }
                else if (exchangeId == "4")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    using (var client = new ExchangePoloniexAPI())
                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Poloniex.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Poloniex.ApiSecret.ToSecureString();

                        var result = await client.PlaceOrderAsync(new ExchangeOrderRequest
                        {
                            Amount = Decimal.Parse(quantity),
                            Price = Decimal.Parse(price),
                            Symbol = market,
                            OrderType = ExchangeSharp.OrderType.Limit,
                            IsBuy = false
                        });

                        return new ExchangeApiResponse<PlaceSellOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = result.ToPlaceSellOrderViewModel(),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                else if (exchangeId == "5")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    using (var client = new ExchangeKucoinAPI())
                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Kucoin.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Kucoin.ApiSecret.ToSecureString();

                        var result = await client.PlaceOrderAsync(new ExchangeOrderRequest
                        {
                            Amount = Decimal.Parse(quantity),
                            Price = Decimal.Parse(price),
                            Symbol = market,
                            OrderType = ExchangeSharp.OrderType.Limit,
                            IsBuy = false
                        });

                        return new ExchangeApiResponse<PlaceSellOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = result.ToPlaceSellOrderViewModel(),
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                /*else if (exchangeId == "5")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    gateio.api.API.SetKey(
                       this._exchangeApiSetting.Value.Gateio.ApiKey,
                       this._exchangeApiSetting.Value.Gateio.ApiSecret);

                    var orderId = await gateio.api.API.SellAsync(new gateio.api.Model.OrderReq
                    {
                        CurrencyPair = market,
                        Amount = Decimal.Parse(quantity, System.Globalization.NumberStyles.Float),
                        Rate = Decimal.Parse(price, System.Globalization.NumberStyles.Float)
                    });

                    if (orderId != null)
                    {
                        return new ExchangeApiResponse<PlaceSellOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = new PlaceSellOrderViewModel { orderId = orderId },
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }*/
            }
            catch (Exception ex)
            {
                /*return new ExchangeApiResponse<OrderBookViewModel>
                {
                    ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                };*/
                return new ExchangeApiResponse<PlaceSellOrderViewModel>
                {
                    ErrorMessage = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest,
                    Timestamp = DateTime.Now
                };
            }

            return new ExchangeApiResponse<PlaceSellOrderViewModel>
            {
                ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                StatusCode = HttpStatusCode.BadRequest,
                Timestamp = DateTime.Now
            };

        }


        public async Task<ExchangeApiResponse<CancelOrderViewModel>> CancelOrder(string exchangeId, string orderId, string baseCurrency = default(string), string quoteCurrency = default(string))
        {
            try
            {
                var apiMethod = ExchangeApiMethodEnum.CancelOrder;
                var uri = this.GetBaseUrl(exchangeId.ToString(), apiMethod);
                var httpMethod = HttpMethod.Delete;

                string market = baseCurrency + quoteCurrency;

                if (exchangeId == "0")
                {
                    uri += "&";
                    market = quoteCurrency + "-" + baseCurrency;

                    return await this.Call<CancelOrderViewModel>(
                    exchangeId.ToString(),
                    uri,
                    httpMethod,
                    apiMethod,
                    Tuple.Create("uuid", orderId)
                    );
                }
                else if (exchangeId == "1")
                {
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        quoteCurrency = "USDT";
                    }
                    market = (baseCurrency + quoteCurrency).ToUpper();
                    return await this.Call<CancelOrderViewModel>(
                        exchangeId.ToString(),
                        uri,
                        httpMethod,
                        apiMethod,
                        Tuple.Create("symbol", market),
                        Tuple.Create("orderId", orderId)
                        );
                }
                else if (exchangeId == "2")
                {
                    httpMethod = HttpMethod.Post;
                    return await this.Call<CancelOrderViewModel>(
                        exchangeId.ToString(),
                        uri + "?",
                        httpMethod,
                        apiMethod,
                        Tuple.Create("txid", orderId)
                        );
                }
                else if (exchangeId == "3")
                {
                    httpMethod = HttpMethod.Post;
                    market = baseCurrency + quoteCurrency;
                    return await this.Call<CancelOrderViewModel>(
                        exchangeId.ToString(),
                        uri + "?",
                        httpMethod,
                        apiMethod,
                        Tuple.Create("id", orderId)
                        );
                }
                else if (exchangeId == "4")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    using (var client = new ExchangePoloniexAPI())
                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Poloniex.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Poloniex.ApiSecret.ToSecureString();

                        await client.CancelOrderAsync(orderId);

                        return new ExchangeApiResponse<CancelOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = new CancelOrderViewModel { orderCancelled = true },
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                else if (exchangeId == "5")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    using (var client = new ExchangeKucoinAPI())
                    {
                        client.PublicApiKey = _exchangeApiSetting.Value.Kucoin.ApiKey.ToSecureString();
                        client.PrivateApiKey = _exchangeApiSetting.Value.Kucoin.ApiSecret.ToSecureString();

                        await client.CancelOrderAsync(orderId);

                        return new ExchangeApiResponse<CancelOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = new CancelOrderViewModel { orderCancelled = true },
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }
                /*else if (exchangeId == "5")
                {
                    market = ExchangeMarketSymbolParser.Parse(exchangeId, baseCurrency, quoteCurrency);

                    gateio.api.API.SetKey(
                       this._exchangeApiSetting.Value.Gateio.ApiKey,
                       this._exchangeApiSetting.Value.Gateio.ApiSecret);

                    var result = gateio.api.API.CancelOrderAsync(new gateio.api.Model.CancelOrderReq
                    {
                        CurrencyPair = market,
                        OrderNo = orderId
                    });
                    if (result.IsCompletedSuccessfully)
                    {
                        return new ExchangeApiResponse<CancelOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = new CancelOrderViewModel { orderCancelled = true },
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                    else
                    {
                        return new ExchangeApiResponse<CancelOrderViewModel>
                        {
                            ErrorMessage = null,
                            Result = new CancelOrderViewModel { orderCancelled = false },
                            StatusCode = HttpStatusCode.OK,
                            Timestamp = DateTime.Now
                        };
                    }
                }*/
            }
            catch (Exception ex)
            {
                return new ExchangeApiResponse<CancelOrderViewModel>
                {
                    ErrorMessage = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest,
                    Timestamp = DateTime.Now
                };
            }


            return new ExchangeApiResponse<CancelOrderViewModel>
            {
                ErrorMessage = $"Exchange with Id '{exchangeId}' has not been found.",
                StatusCode = HttpStatusCode.BadRequest,
                Timestamp = DateTime.Now
            };
        }





        private async Task<ExchangeApiResponse<TApiResponse>> Call<TApiResponse>(string exchangeId, string uri, HttpMethod httpMethod, ExchangeApiMethodEnum method, params Tuple<string, string>[] parameters)
            where TApiResponse : ApiResponse
        {

            ExchangeAPI exchangeAPI = this.GetExchangeAPI(exchangeId);
            /*string uri = string.Format(
                GetBaseUrl(exchangeId, method, out exchangeAPI)
                );*/

            //Append params to uri.
            if (parameters.Length != 0)
            {
                var extraParameters = new StringBuilder();
                extraParameters.Append(parameters[0].Item1 + "=" + parameters[0].Item2);
                for (int i = 1; i < parameters.Length; i++)
                {
                    extraParameters.Append("&" + parameters[i].Item1 + "=" + parameters[i].Item2);
                }

                if (extraParameters.Length > 0)
                {
                    uri = uri + extraParameters.ToString();
                }
            }

            //The 'using' will help to prevent memory leaks.1111
            using (ExchangeDelegatingHandler exchangeDelegatingHandler = new ExchangeDelegatingHandler(exchangeAPI, GetHashFunction(exchangeId), exchangeId))
            {
                using (HttpClient client = HttpClientFactory.Create(exchangeDelegatingHandler))
                {
                    var requestMessage = new HttpRequestMessage(httpMethod, uri);
                    using (HttpResponseMessage res = await client.SendAsync(requestMessage))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var exchangeApiResponse = await ExchangeResponseWrapper.ParseResponse(exchangeId, method, res);

                            return new ExchangeApiResponse<TApiResponse>
                            {
                                ErrorMessage = exchangeApiResponse.ErrorMessage,
                                StatusCode = exchangeApiResponse.StatusCode,
                                Result = exchangeApiResponse.Result as TApiResponse,
                                Timestamp = exchangeApiResponse.Timestamp
                            };
                            //throw new KeyNotFoundException();
                        }
                    }
                }
            }
        }


        private string GetHashFunction(string exchangeId)
        {
            switch (exchangeId)
            {
                case "0":
                    return ExchangeDelegatingHandler.HASH_HMAC512;
                case "1":
                    return ExchangeDelegatingHandler.HASH_HMAC256;
                case "2":
                    return ExchangeDelegatingHandler.HASH_HMAC256;
                case "3":
                    return ExchangeDelegatingHandler.HASH_HMAC256;
                /*case "5":
                    return ExchangeDelegatingHandler.HASH_HMAC512;*/
                default:
                    return ExchangeDelegatingHandler.HASH_HMAC512;
            }

        }


        private string GetBaseUrl(string exchangeId, ExchangeApiMethodEnum method)
        {
            string apiKey = string.Empty;
            string apiSecret = string.Empty;
            string baseUrl = string.Empty;
            string nonce = DateTime.Now.Ticks.ToString();

            if (exchangeId == "0")
            {
                apiKey = _exchangeApiSetting.Value.Bittrex.ApiKey;
                apiSecret = _exchangeApiSetting.Value.Bittrex.ApiSecret;

                switch (method)
                {
                    case ExchangeApiMethodEnum.GetMarket:
                        return "https://bittrex.com/api/v1.1/public/getmarket" + "?apikey=" + apiKey;

                    case ExchangeApiMethodEnum.GetOpenOrders:
                        baseUrl = "https://bittrex.com/api/v1.1/market/getopenorders" + "?apikey=" + apiKey + "&nonce=" + nonce;
                        break;
                    case ExchangeApiMethodEnum.GetBalances:
                        baseUrl = "https://bittrex.com/api/v1.1/account/getbalances" + "?apikey=" + apiKey + "&nonce=" + nonce;
                        break;
                    case ExchangeApiMethodEnum.PlaceBuyOrder:
                        baseUrl = "https://bittrex.com/api/v1.1/market/buylimit" + "?apikey=" + apiKey + "&nonce=" + nonce;
                        break;
                    case ExchangeApiMethodEnum.PlaceSellOrder:
                        baseUrl = "https://bittrex.com/api/v1.1/market/selllimit" + "?apikey=" + apiKey + "&nonce=" + nonce;
                        break;
                    case ExchangeApiMethodEnum.CancelOrder:
                        baseUrl = "https://bittrex.com/api/v1.1/market/cancel" + "?apikey=" + apiKey + "&nonce=" + nonce;
                        break;

                    default:
                        break;
                }
            }
            else if (exchangeId == "1")
            {
                apiKey = _exchangeApiSetting.Value.Binance.ApiKey;
                apiSecret = _exchangeApiSetting.Value.Binance.ApiSecret;

                switch (method)
                {
                    case ExchangeApiMethodEnum.GetMarket:
                        baseUrl = "https://api.binance.com/api/v1/depth";
                        break;
                    case ExchangeApiMethodEnum.GetOpenOrders:
                        baseUrl = "https://api.binance.com/api/v3/openOrders";
                        break;
                    case ExchangeApiMethodEnum.GetBalances:
                        baseUrl = "https://api.binance.com/api/v3/account";
                        break;
                    case ExchangeApiMethodEnum.PlaceBuyOrder:
                        baseUrl = "https://api.binance.com/api/v3/order?side=BUY&type=LIMIT&timeInForce=GTC&icebergQty=0";
                        break;
                    case ExchangeApiMethodEnum.PlaceSellOrder:
                        baseUrl = "https://api.binance.com/api/v3/order?side=SELL&type=LIMIT&timeInForce=GTC&icebergQty=0";
                        break;
                    case ExchangeApiMethodEnum.CancelOrder:
                        baseUrl = "https://api.binance.com/api/v3/order?";
                        break;

                    default:
                        break;
                }
            }
            else if (exchangeId == "2")
            {
                apiKey = _exchangeApiSetting.Value.Kraken.ApiKey;
                apiSecret = _exchangeApiSetting.Value.Kraken.ApiSecret;

                switch (method)
                {
                    case ExchangeApiMethodEnum.GetMarket:
                        baseUrl = "https://api.kraken.com/0/public/Depth";
                        break;
                    case ExchangeApiMethodEnum.GetOpenOrders:
                        baseUrl = "https://api.kraken.com/0/private/OpenOrders";
                        break;
                    case ExchangeApiMethodEnum.GetBalances:
                        baseUrl = "https://api.kraken.com/0/private/Balance";
                        break;
                    case ExchangeApiMethodEnum.PlaceBuyOrder:
                        baseUrl = "https://api.kraken.com/0/private/AddOrder?type=buy&ordertype=limit";
                        break;
                    case ExchangeApiMethodEnum.PlaceSellOrder:
                        baseUrl = "https://api.kraken.com/0/private/AddOrder?type=sell&ordertype=limit";
                        break;
                    case ExchangeApiMethodEnum.CancelOrder:
                        baseUrl = "https://api.kraken.com/0/private/CancelOrder";
                        break;

                    default:
                        break;
                }
            }
            else if (exchangeId == "3")
            {
                apiKey = _exchangeApiSetting.Value.Bitstamp.ApiKey;
                apiSecret = _exchangeApiSetting.Value.Bitstamp.ApiSecret;

                switch (method)
                {
                    case ExchangeApiMethodEnum.GetMarket:
                        baseUrl = "	https://www.bitstamp.net/api/v2/balance/";
                        break;
                    case ExchangeApiMethodEnum.GetOpenOrders:
                        baseUrl = "https://www.bitstamp.net/api/v2/open_orders/all/";
                        break;
                    case ExchangeApiMethodEnum.GetBalances:
                        baseUrl = "https://www.bitstamp.net/api/v2/balance/";
                        break;
                    case ExchangeApiMethodEnum.PlaceBuyOrder:
                        baseUrl = "https://www.bitstamp.net/api/v2/buy/";
                        break;
                    case ExchangeApiMethodEnum.PlaceSellOrder:
                        baseUrl = "https://www.bitstamp.net/api/v2/sell/";
                        break;
                    case ExchangeApiMethodEnum.CancelOrder:
                        baseUrl = "https://www.bitstamp.net/api/v2/cancel_order/";
                        break;

                    default:
                        break;
                }

            }
            /*else if (exchangeId == "5")
            {
                apiKey = _exchangeApiSetting.Value.Bitstamp.ApiKey;
                apiSecret = _exchangeApiSetting.Value.Bitstamp.ApiSecret;

                switch (method)
                {
                    case ExchangeApiMethodEnum.GetMarket:
                        baseUrl = "	https://www.bitstamp.net/api/v2/balance/";
                        break;
                    case ExchangeApiMethodEnum.GetOpenOrders:
                        baseUrl = "https://api.gate.io/api2/1/private/openOrders";
                        break;
                    case ExchangeApiMethodEnum.GetBalances:
                        baseUrl = "https://api.gate.io/api2/1/private/balances";
                        break;
                    case ExchangeApiMethodEnum.PlaceBuyOrder:
                        baseUrl = "https://api.gate.io/api2/1/private/buy";
                        break;
                    case ExchangeApiMethodEnum.PlaceSellOrder:
                        baseUrl = "https://api.gate.io/api2/1/private/sell";
                        break;
                    case ExchangeApiMethodEnum.CancelOrder:
                        baseUrl = "https://api.gate.io/api2/1/private/cancelOrder";
                        break;

                    default:
                        break;
                }
                return baseUrl;
            }*/
            return baseUrl;
        }


        private ExchangeAPI GetExchangeAPI(string exchangeId)
        {
            switch (exchangeId)
            {
                case "0":
                    return _exchangeApiSetting.Value.Bittrex;
                case "1":
                    return _exchangeApiSetting.Value.Binance;
                case "2":
                    return _exchangeApiSetting.Value.Kraken;
                case "3":
                    return _exchangeApiSetting.Value.Bitstamp;
                /*case "5":
                    return _exchangeApiSetting.Value.Gateio;*/
                default:
                    return null;
            }
        }


        private int GetDragonExSymbolId(string baseCurrency, string quoteCurrency)
        {
            switch (baseCurrency + quoteCurrency)
            {
                case "BTCUSDT":
                    return 101;
                case "ETHUSDT":
                    return 103;
                default:
                    return 0;
            }
        }


    }
}
