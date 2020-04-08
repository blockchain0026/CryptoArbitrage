using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeAssetInfoUpdatedIntegrationEventHandler : IIntegrationEventHandler<ExchangeAssetInfoUpdatedIntegrationEvent>
    {
        private readonly IExchangeRepository _exchangeRepository;
        public ExchangeAssetInfoUpdatedIntegrationEventHandler(IExchangeRepository exchangeRepository)
        {
            _exchangeRepository = exchangeRepository;
        }

        public Task Handle(ExchangeAssetInfoUpdatedIntegrationEvent @event)
        {
            try
            {
           
                var exchange = this._exchangeRepository.GetAsync(@event.ExchangeId).Result;
                if (exchange != null)
                {
                    exchange.UpdateAssetBalance(
                        @event.AssetSimbol.ToUpper() == "USDT" ? "USD" : @event.AssetSimbol.ToUpper(),
                        @event.AvailableBalance
                        );
                    _exchangeRepository.Update(exchange);
                }
     
                return Task.FromResult(0);
            }
            catch(Exception ex)
            {
                Debug.Write("Handle Event: ExchangeAssetInfoUpdatedIntegrationEvent." +
                   "Result: Failure." +
                   "Error Message: " + ex.Message
                   );
                throw ex;
            }
        }
    }
}
