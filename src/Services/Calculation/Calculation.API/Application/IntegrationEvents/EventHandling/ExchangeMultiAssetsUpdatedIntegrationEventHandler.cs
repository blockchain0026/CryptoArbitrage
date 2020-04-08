using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeMultiAssetsUpdatedIntegrationEventHandler : IIntegrationEventHandler<ExchangeMultiAssetsUpdatedIntegrationEvent>
    {
        private readonly IExchangeRepository _exchangeRepository;
        public ExchangeMultiAssetsUpdatedIntegrationEventHandler(IExchangeRepository exchangeRepository)
        {
            _exchangeRepository = exchangeRepository;
        }

        public async Task Handle(ExchangeMultiAssetsUpdatedIntegrationEvent @event)
        {
            
            var exchange = await this._exchangeRepository.GetAsync(@event.ExchangeId);
            if (exchange != null)
            {
                foreach(var asset in @event.ExchangeAssets.assets)
                {
                    exchange.UpdateAssetBalance(
                        asset.assetsymbol.ToUpper() == "USDT" ? "USD" : asset.assetsymbol.ToUpper(),
                        asset.availablebalance
                        );
                }
       
                _exchangeRepository.Update(exchange);
            }
        }
    }
}
