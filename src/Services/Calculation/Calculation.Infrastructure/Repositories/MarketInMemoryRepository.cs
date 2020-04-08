using CryptoArbitrage.Services.Calculation.Domain.Model.Markets;
using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Infrastructure.Repositories
{
    public class MarketInMemoryRepository : IMarketRepository
    {
        private readonly IDictionary<MarketId, Market> _entities;
        private readonly IMediator _mediator;

        IUnitOfWork IRepository<Market>.UnitOfWork => throw new NotImplementedException();


        public MarketInMemoryRepository(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._entities = new Dictionary<MarketId, Market>();
            this.Initialize();
        }


        public Market Add(Market market)
        {
            if (market == null)
                throw new ArgumentNullException(nameof(market));
            if (market.MarketId == null)
                throw new ArgumentNullException(nameof(market.MarketId));

            if (!this._entities.ContainsKey(market.MarketId))
            {
                lock (this._entities)
                {
                    this._entities.Add(market.MarketId, market);
                    return market;
                }
            }
            else
            {
                throw new InvalidOperationException("Market with key " + market.MarketId.BaseCurrency + market.MarketId.QuoteCurrency + " is already exist." +
                    " Try use update function instead.");
            }

        }

        public async Task<Market> GetAsync(MarketId marketId)
        {
            Market market = null;


            if (!_entities.TryGetValue(marketId, out market))
            {
                throw new KeyNotFoundException($"Market {marketId.BaseCurrency}/{marketId.QuoteCurrency} not found.");
            }


            return market;
        }

        public async Task Update(Market market)
        {
            if (market == null)
                throw new ArgumentNullException(nameof(market));
            if (market.MarketId == null)
                throw new ArgumentNullException(nameof(market.MarketId));


            if (!_entities.TryGetValue(market.MarketId, out Market marketToUpdate))
            {
                throw new KeyNotFoundException($"Market {market.MarketId.BaseCurrency}/{market.MarketId.QuoteCurrency} not found.");
            }

            //var existingMarket = GetAsync(market.MarketId).Result;

            if (marketToUpdate != null)
            {
                //existingMarket = market;
                _entities[market.MarketId] = market;
                await this.DispatchEvents();
            }
        }

        private async Task DispatchEvents()
        {
            await _mediator.DispatchDomainEventsAsync(this._entities.Values);
        }

        private void Initialize()
        {
            var marketBTCUSD = new Market("BTC", "USD", 10);
            var marketETHUSD = new Market("ETH", "USD", 10);

            this.Add(marketBTCUSD);
            this.Add(marketETHUSD);
        }

    }
}
