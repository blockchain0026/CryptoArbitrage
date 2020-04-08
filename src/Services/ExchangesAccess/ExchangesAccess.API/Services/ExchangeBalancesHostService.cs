using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Services
{
    public class ExchangeBalancesHostService : IHostedService
    {

        private CancellationTokenSource _cts;
        private Task _executingTask;

        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IEventBus _eventBus;


        public ExchangeBalancesHostService(IExchangeApiClient exchangeApiClient, IEventBus eventBus)
        {
            _exchangeApiClient = exchangeApiClient ?? throw new ArgumentNullException(nameof(exchangeApiClient));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = Task.Run(async () =>
            {
                //Keep detecting until shutdown.
                while (!_cts.IsCancellationRequested)
                {
                    /*await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);*/
                }
            });

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }


      

        public static void UpdateBalanceInfo(int exchangeId, string symbol, decimal balance, decimal available, decimal pending, IEventBus eventBus)
        {
            /*eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        exchangeId,
                        symbol,
                        balance,
                        available,
                        pending
                        ));*/

            eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        exchangeId,
                        symbol,
                        0,
                        0,
                        0
                        ));
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();

            // 等待后台服务的停止，而 ASP.NET Core 大约会等待5秒钟（可在上面介绍的UseShutdownTimeout方法中配置），如果还没有执行完会发送取消信号，以防止无限的等待下去。
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

    }
}
