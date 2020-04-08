using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Calculation.API.Extensions;
using CryptoArbitrage.Services.Calculation.API.IntegrationEvents.Events;
using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using CryptoArbitrage.Services.Calculation.Domain.Model.Markets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.IntegrationEvents.EventHandling
{
    public class ExchangeOrderBookUpdatedIntegrationEventHandler : IIntegrationEventHandler<ExchangeOrderBookUpdatedIntegrationEvent>
    {
        private readonly IMarketRepository _marketRepository;
        private readonly IExchangeRepository _exchangeRepository;
        public ExchangeOrderBookUpdatedIntegrationEventHandler(IMarketRepository marketRepository, IExchangeRepository exchangeRepository)
        {
            _marketRepository = marketRepository;
            _exchangeRepository = exchangeRepository;
        }

        public async Task Handle(ExchangeOrderBookUpdatedIntegrationEvent @event)
        {
            try
            {
                var quoteCurrency = @event.QuoteCurrency.ToUpper() == "USDT" ? "USD" : @event.QuoteCurrency.ToUpper();
                var baseCurrency = @event.BaseCurrency.ToUpper();

                var marketId = new MarketId(baseCurrency, quoteCurrency);

                var market = await _marketRepository.GetAsync(marketId);
                var exchange = await _exchangeRepository.GetAsync(@event.ExchangeId);

                var bids = @event.OrderBook.bids.AsEnumerable().Take(1).ToOrderPriceAndQuantitys();
                var asks = @event.OrderBook.asks.AsEnumerable().Take(1).ToOrderPriceAndQuantitys();

                market.UpdateExchangeMarket(exchange, bids, asks);
                await _marketRepository.Update(market);
            }

            catch(Exception ex)
            {
                Debug.Write("Handle Event: ExchangeOrderBookUpdatedIntegrationEvent." +
                    "Result: Failure." +
                    "Error Message: "+ex.Message
                    );
                throw ex;
            }

            //var askList = @event.OrderBook.asks;
        }
    }
}
