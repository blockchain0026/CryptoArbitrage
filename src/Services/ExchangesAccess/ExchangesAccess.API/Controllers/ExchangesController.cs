using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Commands;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using CryptoArbitrage.Services.ExchangesAccess.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ExchangesAccess.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangesController : Controller
    {
        /* private readonly IMediator _mediator;
         private readonly IExchangeQueries _exchangeQueries;
         private readonly IIdentityService _identityService;*/
        public const string METHOD_GETMARKET = "method_getmarket";
        public const string METHOD_GETOPENORDER = "method_getopenorder";
        public const string METHOD_PLACEBUYORDER = "method_placebuyorder";
        public const string METHOD_PLACESELLORDER = "method_placesellorder";
        public const string METHOD_CANCELORDER = "method_cancelorder";
        public const string METHOD_GETBALANCES = "method_getbalance";


        private readonly IOptions<ExchangeApiSetting> _exchangeApiSetting;
        private readonly IWebsocketService _websocketService;
        private readonly IExchangeQueries _exchangeQueries;
        private readonly IExchangeApiClient _exchangeApiClient;
        //private readonly IEventBus _eventBus;

        public ExchangesController(IOptions<ExchangeApiSetting> exchagneApiSetting, IWebsocketService websocketService, IExchangeQueries exchangeQueries, IExchangeApiClient exchangeApiClient, IEventBus eventBus)
        {
            _exchangeApiSetting = exchagneApiSetting;
            _websocketService = websocketService;
            _exchangeQueries = exchangeQueries;
            _exchangeApiClient = exchangeApiClient;
            //_eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            /*_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));*/
        }


        /// <summary>
        /// 查詢交易所的市場委託
        /// </summary>
        /// <param name="exchangeId">
        /// 欲查詢之交易所ID：
        /// 0 = Bittrex
        /// 1 = Binance
        /// 2 = Kraken
        /// 3 = Bitstamp
        /// 4 = Poloniex
        /// </param>
        /// 
        /// <param name="baseCurrency">
        /// 欲查詢的基礎貨幣，如 "BTC" 。 
        /// </param>
        /// <param name="quoteCurrency">
        /// 欲查詢的報價貨幣，如 "USD" 。 
        /// </param>
        /// 
        // GET: api/<controller>/GetOrderBook?exchangeId=0&baseCurrency=BTC&quoteCurrency=USD
        [Route("GetOrderBook")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrderBook(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            try
            {
                var orderBook = await _exchangeApiClient.GetOrderBook(exchangeId, baseCurrency, quoteCurrency);
                if (orderBook != null)
                    return Ok(orderBook);
                throw new KeyNotFoundException();

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        /// <summary>
        /// 查詢交易所的市場委託
        /// </summary>
        /// <param name="exchangeId">
        /// 欲查詢之交易所ID：
        /// 0 = Bittrex
        /// 1 = Binance
        /// 2 = Kraken
        /// 3 = Bitstamp
        /// 4 = Poloniex
        /// </param>
        /// 
        /// <param name="baseCurrency">
        /// 欲查詢的基礎貨幣，如 "BTC" 。 
        /// </param>
        /// <param name="quoteCurrency">
        /// 欲查詢的報價貨幣，如 "USD" 。 
        /// </param>
        /// 
        // GET: api/<controller>/GetOrderBook?exchangeId=0&baseCurrency=BTC&quoteCurrency=USD
        [Route("GetOrderBookRealTime")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrderBookRealTime(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            try
            {
                string uri = string.Empty;
                HttpMethod httpMethod = HttpMethod.Get;

                baseCurrency = baseCurrency.ToUpper();
                quoteCurrency = quoteCurrency.ToUpper();

                var data = await _exchangeQueries.GetMarketAsync(exchangeId, baseCurrency, quoteCurrency);

                if (data != null)
                    return Ok(data);

                throw new KeyNotFoundException();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        /// <summary>
        /// 查詢下單資訊
        /// </summary>
        /// 
        /// <remarks>
        /// 欲查詢特定交易對，則須同時傳入 baseCurrency 與 quoteCurrency。
        /// 若只傳入 exchangeId，則返還所有未完全成交的委託單。
        /// 
        /// </remarks>
        /// <param name="exchangeId">
        /// 欲查詢之交易所ID：
        /// 0 = Bittrex
        /// 1 = Binance
        /// 2 = Kraken
        /// 3 = Bitstamp
        /// 4 = Poloniex
        /// </param>
        /// 
        /// <param name="baseCurrency">
        /// 欲查詢的基礎貨幣，如 "BTC" 。 
        /// </param>
        /// <param name="quoteCurrency">
        /// 欲查詢的報價貨幣，如 "USD" 。 
        /// </param>
        /// 
        // GET: api/<controller>/GetOpenOrder?exchangeId=0
        [Route("GetOpenOrder")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOpenOrders(int exchangeId, string baseCurrency = default(string), string quoteCurrency = default(string))
        {
            try
            {

                var openOrders = await _exchangeApiClient.GetOpenOrder(exchangeId.ToString(), baseCurrency, quoteCurrency);
                if (openOrders != null)
                {
                    return Ok(openOrders);
                }


                throw new KeyNotFoundException();

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        /// <summary>
        /// 查詢帳戶餘額
        /// </summary>
        /// 
        /// <remarks>
        /// 欲查詢特定交易對，則須同時傳入 baseCurrency 與 quoteCurrency。
        /// 若只傳入 exchangeId，則返還所有貨幣餘額，並用預設幣種計價。
        /// </remarks>
        /// 
        /// <param name="exchangeId">
        /// 欲查詢之交易所ID：
        /// 0 = Bittrex
        /// 1 = Binance
        /// 2 = Kraken
        /// 3 = Bitstamp
        /// 4 = Poloniex
        /// </param>
        /// 
        /// <param name="baseCurrency">
        /// 欲查詢的基礎貨幣，如 "BTC" 。 
        /// </param>
        /// <param name="quoteCurrency">
        /// 欲查詢的報價貨幣，如 "USD" 。 
        /// </param>
        /// 
        [Route("GetBalance")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBalances(int exchangeId, string baseCurrency = default(string), string quoteCurrency = default(string))
        {
            try
            {

                var balances = await _exchangeApiClient.GetBalances(exchangeId.ToString(), baseCurrency, quoteCurrency);
                if (balances != null)
                {
                    return Ok(balances);
                }
                throw new KeyNotFoundException();

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

        }


        /// <summary>
        /// 執行買入限價委託
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Exchanges/BuyOrder
        ///     {
        ///        "exchangeId": "0",
        ///        "baseCurrency": "eth",
        ///        "quoteCurrency": "usd",
        ///        "quantity":"0.1",
        ///        "price":"250"
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// </param>
        /// 
        [Route("BuyOrder")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PlaceBuyOrder([FromBody]PlaceBuyOrderCommand command)
        {
            try
            {
                string exchangeId = command.ExchangeId;
                string baseCurrency = command.BaseCurrency;
                string quoteCurrency = command.QuoteCurrency;

                //string market = command.BaseCurrency + command.QuoteCurrency;
                string quantity = command.Quantity;
                string price = command.Price;

                var buyOrderViewModel = await _exchangeApiClient.PlaceBuyOrder(exchangeId, baseCurrency, quoteCurrency, quantity, price);
                if (buyOrderViewModel != null)
                {
                    return Ok(buyOrderViewModel);
                }
                throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>
        /// 執行賣出限價委託
        /// </summary>
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Exchanges/SellOrder
        ///     {
        ///        "exchangeId": "0",
        ///        "baseCurrency": "eth",
        ///        "quoteCurrency": "usd",
        ///        "quantity":"0.1",
        ///        "price":"320"
        ///     } 
        ///     
        /// </remarks>
        /// 
        /// <param name="command">
        /// </param>
        /// 
        [Route("SellOrder")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PlaceSellOrder([FromBody]PlaceSellOrderCommand command)
        {
            try
            {
                string exchangeId = command.ExchangeId;
                string baseCurrency = command.BaseCurrency;
                string quoteCurrency = command.QuoteCurrency;

                string quantity = command.Quantity;
                string price = command.Price;

                var sellOrderViewModel = await _exchangeApiClient.PlaceSellOrder(exchangeId, baseCurrency, quoteCurrency, quantity, price);
                if (sellOrderViewModel != null)
                {
                    return Ok(sellOrderViewModel);
                }


                throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>
        /// 取消委託單。
        /// </summary>
        /// 
        /// <param name="exchangeId">
        /// 欲查詢之交易所ID：
        /// 0 = Bittrex
        /// 1 = Binance
        /// 2 = Kraken
        /// 3 = Bitstamp
        /// 4 = Poloniex
        /// </param>   
        /// 
        /// <param name="orderId">
        /// 欲取消的委託單ID，可調用 /Exchange/GetOpenOrder 查詢 orderId。
        /// </param> 
        /// 
        /// <param name="baseCurrency">
        /// 特定交易所需描述取消的委託單交易對。
        /// 需傳入此數值的交易所：
        /// 1 = Binance
        /// </param>
        /// 
        /// <param name="quoteCurrency">
        /// 特定交易所需描述取消的委託單交易對。
        /// 需傳入此數值的交易所：
        /// 1 = Binance
        /// </param>
        [Route("CancelOrder")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrder(int exchangeId, string orderId, string baseCurrency = default(string), string quoteCurrency = default(string))
        {
            var cancelOrderViewModel = await _exchangeApiClient.CancelOrder(exchangeId.ToString(), orderId, baseCurrency, quoteCurrency);
            if (cancelOrderViewModel != null)
                return Ok(cancelOrderViewModel);

            return NotFound();
        }


        /// <summary>
        /// 取消委託單。
        /// </summary>
        /// 
        /// <param name="exchangeId">
        /// 欲查詢之交易所ID：
        /// 0 = Bittrex
        /// 1 = Binance
        /// 2 = Kraken
        /// 3 = Bitstamp
        /// 4 = Poloniex
        /// </param>   
        /// 
        /// <param name="orderId">
        /// 欲取消的委託單ID，可調用 /Exchange/GetOpenOrder 查詢 orderId。
        /// </param> 
        /// 
        /// <param name="baseCurrency">
        /// 特定交易所需描述取消的委託單交易對。
        /// 需傳入此數值的交易所：
        /// 1 = Binance
        /// </param>
        /// 
        /// <param name="quoteCurrency">
        /// 特定交易所需描述取消的委託單交易對。
        /// 需傳入此數值的交易所：
        /// 1 = Binance
        /// </param>
        [Route("CheckExchangeServerTime")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckExchangeServerTime(int exchangeId)
        {
            if (exchangeId == 1)
            {
                var uri = "https://api.binance.com//api/v1/time";
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(uri))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var formObject = await content.ReadAsStringAsync();

                            JObject response;

                            response = JObject.Parse(formObject);
                            if (response["serverTime"] != null)
                            {
                                var resList = new List<KeyValuePair<string, string>>();
                                resList.Add(new KeyValuePair<string, string>(
                                    "Exchange Server Time",
                                    response["serverTime"].ToString()
                                    ));
                                resList.Add(new KeyValuePair<string, string>(
                                    "Local Server Time",
                                    //DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString()
                                    DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
                                    ));
                                return Ok(Json(resList));
                            }
                        }
                    }
                }
            }

            return NotFound();
        }

    }
}