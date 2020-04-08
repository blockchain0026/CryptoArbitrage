using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.EventHandling
{
    public class TimeForUpdateBalanceIntegrationEventHandler : IIntegrationEventHandler<TimeForUpdateBalanceIntegrationEvent>
    {
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IEventBus _eventBus;

        public TimeForUpdateBalanceIntegrationEventHandler(IExchangeApiClient exchangeApiClient, IEventBus eventBus)
        {
            _exchangeApiClient = exchangeApiClient ?? throw new ArgumentNullException(nameof(exchangeApiClient));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task Handle(TimeForUpdateBalanceIntegrationEvent @event)
        {
            try
            {
                var lengh = 2;
                for (int i = 0; i < lengh; i++)
                {
                    _eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        @event.ExchangeId,
                        i.ToString(),
                        0,
                        0,
                        0
                        ));
                }


                /* var balancesViewModel = this._exchangeApiClient.GetBalances(i.ToString(), default(string), default(string)).Result;

                 var assets = new List<ExchangeAssetIntegationModel>();
                 foreach (var asset in balancesViewModel.Result.pairs)
                 {
                     var balance = Decimal.Parse(asset.balance ?? "0", NumberStyles.Float);
                     var available = Decimal.Parse(asset.available ?? "0", NumberStyles.Float);
                     var pending = Decimal.Parse(asset.pending ?? "0", NumberStyles.Float);
                     var symbol = asset.currency.ToUpper();


                     UpdateBalanceInfo(i, symbol, balance, available, pending, this._eventBus);


                 }*/
            }
            catch (Exception)
            {

            }
        }
    }
}
