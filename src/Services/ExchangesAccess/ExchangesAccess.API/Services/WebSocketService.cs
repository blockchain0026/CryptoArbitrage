using Binance.Net;
using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using CryptoArbitrage.Services.ExchangesAccess.API.IntegrationEvents.Events;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoloniexWebSocketsApi;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Services
{
    public class WebsocketService : IWebsocketService
    {
        public delegate void ExchangeCallback(string exchangeId, string baseCurrency, string quoteCurrency, string info);

        private HubConnection _hubConnection { get; }
        private BinanceSocketClient _binanceHubConnection { get; }
        private Pusher _bitstampHubConnection { get; }
        private PoloniexChannel _poloniexHubConnection { get; }
        private WebSocket _kucoinHubConnection { get; }
        private IHubProxy _hubProxy { get; }


        private ExchangeCallback _updateAndDecodeExchangeState { get; }
        private ExchangeCallback _updateExchangeState { get; }
        private ExchangeCallback _updateOrderState { get; }
        private ExchangeCallback _updateBalanceState { get; }


        private IExchangeMarketData _exchangeMarketData { get; }

        private readonly IEventBus _eventBus;




        public WebsocketService(IExchangeMarketData exchangeMarketData, IEventBus eventBus)
        {
            //Injection
            _exchangeMarketData = exchangeMarketData ?? throw new ArgumentNullException(nameof(exchangeMarketData));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            //Set delegates
            _updateAndDecodeExchangeState = CreateDecodeCallback(_exchangeMarketData);
            _updateExchangeState = CreateCallback(_exchangeMarketData, _eventBus);



            //Bittrex
            _hubConnection = new HubConnection("https://socket.bittrex.com/signalr");
            _hubProxy = _hubConnection.CreateHubProxy("c2");
            _hubConnection.Start().Wait();


            //Binance
            _binanceHubConnection = new BinanceSocketClient();


            //Bitstamp
            _bitstampHubConnection = new Pusher("de504dc5763aeef9ff52");
            _bitstampHubConnection.Connect();


            //Poloniex
            _poloniexHubConnection = new PoloniexChannel();
            _poloniexHubConnection.ConnectAsync().Wait();


            //Kucoin
            using (var client = new HttpClient())
            {
                var tokenAcquireUri = "https://kitchen.kucoin.com/v1/bullet/usercenter/loginUser?protocol=websocket&encrypt=true";
                using (HttpResponseMessage res = client.GetAsync(tokenAcquireUri).Result)
                {
                    using (HttpContent content = res.Content)
                    {
                        var jRes = JObject.Parse(
                            content.ReadAsStringAsync().Result
                            );
                        var bulletToken = jRes["data"]["bulletToken"].ToString();

                        _kucoinHubConnection = new WebSocket(
                            "wss://push1.kucoin.com/endpoint?bulletToken=" + bulletToken + "&format=json&resource=api");

                        _kucoinHubConnection.Connect();
                        var success = _kucoinHubConnection.Ping();
                    }
                }


            }


        }


        public void Shutdown() => _hubConnection.Stop();

        // marketName example: "BTC-LTC"
        public async Task<bool> SubscribeToExchangeDeltas(string marketName) => await _hubProxy.Invoke<bool>("SubscribeToExchangeDeltas", marketName);

        // The return of GetAuthContext is a challenge string. Call CreateSignature(apiSecret, challenge)
        // for the response to the challenge, and pass it to Authenticate().
        public async Task<string> GetAuthContext(string apiKey) => await _hubProxy.Invoke<string>("GetAuthContext", apiKey);

        public async Task<bool> Authenticate(string apiKey, string signedChallenge) => await _hubProxy.Invoke<bool>("Authenticate", apiKey, signedChallenge);


        public async Task<bool> StartListening()
        {
            bool success = false;

            #region Bittrex
            while (true)
            {
                if (_hubConnection.State != Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    continue;
                }

                // Register callback for uE (exchange state delta) events
                _hubProxy.On(
                    "uE",
                    exchangeStateDelta => _updateAndDecodeExchangeState?.Invoke("0", "no need", "no need", exchangeStateDelta)
                    );

                success = true;
                break;
            }
            #endregion
            return success;

        }



        public async Task<bool> ListenToMarket(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            baseCurrency = baseCurrency.ToUpper();
            quoteCurrency = quoteCurrency.ToUpper();

            bool retry = true;
            string market = String.Empty;

            switch (exchangeId)
            {
                case "0":
                    market = (quoteCurrency + "-" + baseCurrency).ToUpper();
                    break;
                case "1":
                    market = baseCurrency.ToLower() + quoteCurrency.ToLower();
                    break;
                case "3":
                    market = baseCurrency.ToLower() + quoteCurrency.ToLower();
                    break;
                case "4":
                    market = (quoteCurrency + "_" + baseCurrency).ToUpper();
                    break;
                case "5":
                    market = (baseCurrency + "-" + quoteCurrency).ToUpper();
                    break;
                default:
                    market = String.Empty;
                    break;
            }

            if (exchangeId == "0")
            {

                while (retry)
                {
                    try
                    {
                        retry = !SubscribeToExchangeDeltas(market).Wait(1000);
                    }
                    catch (Exception)
                    {
                        retry = true;
                    }

                }
            }
            else if (exchangeId == "1")
            {
                #region Binance

                while (retry)
                {
                    try
                    {
                        retry = !_binanceHubConnection.SubscribeToPartialBookDepthStream(market, 10,
                            (data) => _updateExchangeState?.Invoke(exchangeId, baseCurrency, quoteCurrency, JsonConvert.SerializeObject(data))
                            ).Success;
                    }
                    catch (Exception)
                    {
                        retry = true;
                    }
                }


                #endregion
            }
            else if (exchangeId == "3")
            {
                Channel orderBooksChannel = null;

                var subscriptionString = market == "btcusd" ? "order_book" : "order_book_" + market;

                orderBooksChannel = _bitstampHubConnection.Subscribe(subscriptionString);

                //retry = !orderBooksChannel.IsSubscribed;


                orderBooksChannel.Bind("data",
                    (data) => _updateExchangeState?.Invoke(exchangeId, baseCurrency, quoteCurrency, JsonConvert.SerializeObject(data)));

                retry = false;
            }
            else if (exchangeId == "4")
            {
                _poloniexHubConnection.MessageArrived += (serializer, message)
                     => _updateExchangeState?.Invoke(
                         JsonConvert.SerializeObject(message).ToString() == "[1010]" ?
                         string.Empty : exchangeId,
                         baseCurrency,
                         quoteCurrency,
                         JsonConvert.SerializeObject(message)
                     );

                await _poloniexHubConnection.SendAsync(new PoloniexCommand() { Channel = TickerSymbol.USDT_BTC, Command = PoloniexCommandType.Subscribe });
                retry = false;

            }
            else if (exchangeId == "5")
            {
                var id = DateTime.UtcNow.Ticks;
                _kucoinHubConnection.Send(JsonConvert.SerializeObject(new { id = id++, type = "subscribe", topic = $"/trade/{market}_TRADE" }));

                _kucoinHubConnection.OnMessage += (sender, e)
                      => _updateExchangeState?.Invoke(
                             exchangeId,
                             baseCurrency,
                             quoteCurrency,
                             e.Data
                             );
                retry = false;
            }
            return !retry;
        }


        public async Task<bool> ListenToBalance(string exchangeId)
        {


            return false;
        }

        // Decode converts Bittrex CoreHub2 socket wire protocol data into JSON.
        // Data goes from base64 encoded to gzip (byte[]) to minifed JSON.
        public static string Decode(string wireData)
        {

            // Step 1: Base64 decode the wire data into a gzip blob
            byte[] gzipData = Convert.FromBase64String(wireData);

            // Step 2: Decompress gzip blob into minified JSON
            using (var decompressedStream = new MemoryStream())
            using (var compressedStream = new MemoryStream(gzipData))
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(decompressedStream);
                decompressedStream.Position = 0;

                using (var streamReader = new StreamReader(decompressedStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static string CreateSignature(string apiSecret, string challenge)
        {
            // Get hash by using apiSecret as key, and challenge as data
            var hmacSha512 = new HMACSHA512(Encoding.ASCII.GetBytes(apiSecret));
            var hash = hmacSha512.ComputeHash(Encoding.ASCII.GetBytes(challenge));
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        static public ExchangeCallback CreateDecodeCallback(IExchangeMarketData exchangeMarketData)
        {
            return (exchangeId, baseCurrency, quoteCurrency, info) =>
            {
                WriteMarketData(exchangeId, baseCurrency, quoteCurrency, Decode(info), exchangeMarketData).Wait();


            };
        }

        static public ExchangeCallback CreateCallback(IExchangeMarketData exchangeMarketData, IEventBus eventBus)
        {
            return (exchangeId, baseCurrency, quoteCurrency, info) =>
            {
                WriteMarketData(exchangeId, baseCurrency, quoteCurrency, info, exchangeMarketData).Wait();

                try
                {
                    if (!String.IsNullOrEmpty(exchangeId))
                    {

                        var @event = new ExchangeOrderBookUpdatedIntegrationEvent(
                                                  Int32.Parse(exchangeId),
                                                  baseCurrency,
                                                  quoteCurrency,
                                                  exchangeMarketData.GetMarketData(exchangeId, baseCurrency, quoteCurrency)
                                                  );

                        eventBus.Publish(@event);
                        //.Publish(new TimeForUpdateBalanceIntegrationEvent());
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine("Publish Event: ExchangeOrderBookUpdatedIntegrationEvent. \n" +
                        "Error message: " + ex.Message);
                }
            };
        }

        private static async Task WriteMarketData(string exchangeId, string baseCurrency, string quoteCurrency, string data, IExchangeMarketData exchangeMarketData)
        {
            await Task.Run(() =>
            {
                exchangeMarketData.WriteMarketData(exchangeId, baseCurrency, quoteCurrency, data);
            });

        }




        public bool IsConnectted(string exchangeId)
        {

            if (exchangeId == "0")
            {
                //return _hubConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected;
                return true;
            }
            else if (exchangeId == "1")
            {
                //Need to change the logic later.
                return true;
            }
            else if (exchangeId == "3")
            {
                return _bitstampHubConnection.State == PusherClient.ConnectionState.Connected;
            }
            else if (exchangeId == "4")
            {
                //Need to change the logic later.
                return true;
            }
            else if(exchangeId=="5")
            {
                return this._kucoinHubConnection.Ping();

            }

            return false;
        }
    }
}
