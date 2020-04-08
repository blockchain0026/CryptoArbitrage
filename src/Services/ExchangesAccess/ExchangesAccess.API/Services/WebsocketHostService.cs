using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Services
{
    public class WebsocketHostService : IHostedService
    {
        private CancellationTokenSource _cts;
        private Task _executingTask;
        private readonly IWebsocketService _websocketService;
        private IOptions<ExchangeApiSetting> _exchangeApiSettings;
        private IExchangeMarketData _exchangeMarketData;
        private IExchangeBalanceData _exchangeBalanceData;
        private IExchangeApiClient _exchangeApiClient;


        public WebsocketHostService(IWebsocketService websocketService, IOptions<ExchangeApiSetting> exchangeApiSetting,
            IExchangeMarketData exchangeMarketData, IExchangeBalanceData exchangeBalanceData,
            IExchangeApiClient exchangeApiClient)
        {
            _websocketService = websocketService;
            _exchangeApiSettings = exchangeApiSetting;
            _exchangeMarketData = exchangeMarketData;
            _exchangeBalanceData = exchangeBalanceData;
            _exchangeApiClient = exchangeApiClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = Task.Run(async () =>
            {
                //this.StartConnection("0").Wait();
                this.StartConnection("1").Wait();
                this.StartConnection("3").Wait();
                this.StartConnection("4").Wait();
                this.StartConnection("5").Wait();

                bool allConnected = false;

                //Keep detecting until shutdown.
                while (!_cts.IsCancellationRequested)
                {

                    #region MarketData
                    allConnected = CheckConnection(out List<string> offlines).Result;

                    if (!allConnected)
                    {
                        foreach (var exchangeId in offlines)
                        {
                            StartConnection(exchangeId).Wait();
                        }
                        offlines = new List<string>();
                    }
                    #endregion


                    #region BalanceData

                    await this.UpdateBalanceData(0);
                    await this.UpdateBalanceData(1);
                    await this.UpdateBalanceData(2);
                    await this.UpdateBalanceData(3);
                    await this.UpdateBalanceData(4);
                    await this.UpdateBalanceData(5);
                    #endregion

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            });

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task<bool> StartConnection(string exchangeId)
        {
            bool success = false;


            if (exchangeId == "0")
            {
                /*if (await _websocketService.StartListening())
                {
                    await _exchangeMarketData.InitializeMarketData(exchangeId, "btc", "usdt");
                    await _exchangeMarketData.InitializeMarketData(exchangeId, "eth", "usdt");

                    if (await _websocketService.ListenToMarket(exchangeId, "btc", "usdt")
                        && await _websocketService.ListenToMarket(exchangeId, "eth", "usdt"))
                    {
                        await _exchangeMarketData.InitializeMarketData(exchangeId, "btc", "usdt");
                        await _exchangeMarketData.InitializeMarketData(exchangeId, "eth", "usdt");
                        success = true;
                    }
                }*/
            }
            else if (exchangeId == "1")
            {
                await _exchangeMarketData.InitializeMarketData(exchangeId, "btc", "usdt");
                await _exchangeMarketData.InitializeMarketData(exchangeId, "eth", "usdt");
                if (await _websocketService.ListenToMarket(exchangeId, "btc", "usdt")
                    && await _websocketService.ListenToMarket(exchangeId, "eth", "usdt"))
                {
                    success = true;
                }
            }
            else if (exchangeId == "3")
            {
                await _exchangeMarketData.InitializeMarketData(exchangeId, "btc", "usd");
                await _exchangeMarketData.InitializeMarketData(exchangeId, "eth", "usd");
                if (await _websocketService.ListenToMarket(exchangeId, "btc", "usd")
                     && await _websocketService.ListenToMarket(exchangeId, "eth", "usd"))
                {
                    success = true;
                }
            }
            else if (exchangeId == "4")
            {
                await _exchangeMarketData.InitializeMarketData(exchangeId, "btc", "usd");
                //await _exchangeMarketData.InitializeMarketData(exchangeId, "eth", "usd");
                if (await _websocketService.ListenToMarket(exchangeId, "btc", "usd")
                    /* && await _websocketService.ListenToMarket(exchangeId, "eth", "usd")*/)
                {
                    success = true;
                }
            }
            else if (exchangeId == "5")
            {
                await _exchangeMarketData.InitializeMarketData(exchangeId, "btc", "usdt");
                //await _exchangeMarketData.InitializeMarketData(exchangeId, "eth", "usd");
                if (await _websocketService.ListenToMarket(exchangeId, "btc", "usdt")
                    /* && await _websocketService.ListenToMarket(exchangeId, "eth", "usd")*/)
                {
                    success = true;
                }
            }
            return success;
        }

        public async Task UpdateBalanceData(int exchangeId)
        {
            try
            {
                var balanceViewModel = await this._exchangeApiClient.GetBalances(exchangeId.ToString(), default(string), default(string));

                foreach (var asset in balanceViewModel.Result.pairs)
                {
                    var symbol = asset.currency.ToUpper();
                    var balance = Decimal.Parse(asset.balance ?? "0", NumberStyles.Float);
                    var available = Decimal.Parse(asset.available ?? "0", NumberStyles.Float);
                    var pending = Decimal.Parse(asset.pending ?? "0", NumberStyles.Float);

                    await this._exchangeBalanceData.WriteBalanceData(exchangeId.ToString(), symbol, balance, available, pending);
                }
                
            }

            catch (Exception ex)
            {
                Debug.WriteLine($"Updating balance data for exchange {exchangeId} from WebsocketHostService failed.");
                Debug.WriteLine("Error Messages:" + ex.Message);
                return;
            }
        }

        public Task<bool> CheckConnection(out List<string> offlineExchangeIds)
        {
            var success = true;
            var offlines = new List<string>();

            for (int i = 0; i <=5; i++)
            {
                //for temp.
                if (i == 2)
                    continue;
                if (!_websocketService.IsConnectted(i.ToString()))
                {
                    offlines.Add(i.ToString());
                    success = false;
                }
            }



            offlineExchangeIds = offlines;
            return Task.FromResult(success);
        }



        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // 发送停止信号，以通知我们的后台服务结束执行。
            _websocketService.Dispose();
            _cts.Cancel();

            // 等待后台服务的停止，而 ASP.NET Core 大约会等待5秒钟（可在上面介绍的UseShutdownTimeout方法中配置），如果还没有执行完会发送取消信号，以防止无限的等待下去。
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

    }
}
