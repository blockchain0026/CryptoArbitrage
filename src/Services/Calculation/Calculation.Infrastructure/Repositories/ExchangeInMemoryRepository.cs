using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Infrastructure.Repositories
{
    public class ExchangeInMemoryRepository : IExchangeRepository
    {
        private readonly IDictionary<int, Exchange> _entities;
        private readonly IMediator _mediator;

        IUnitOfWork IRepository<Exchange>.UnitOfWork => throw new NotImplementedException();


        public ExchangeInMemoryRepository(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._entities = new Dictionary<int, Exchange>();
            this.Initialize();
        }


        public Exchange Add(Exchange exchange)
        {
            if (exchange == null)
                throw new ArgumentNullException(nameof(exchange));
            if (exchange.ExchangeId < 0)
                throw new ArgumentOutOfRangeException(nameof(exchange.ExchangeId));

            try
            {
                this._entities.Add(exchange.ExchangeId, exchange);
                return exchange;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<Exchange> GetAsync(int exchangeId)
        {
            Exchange exchange = null;


            if (!_entities.TryGetValue(exchangeId, out exchange))
            {
                throw new KeyNotFoundException($"Exchange {exchangeId} not found.");
            }


            return exchange;
        }

        public void Update(Exchange exchange)
        {
            if (exchange == null)
                throw new ArgumentNullException(nameof(exchange));
            var exchangeId = exchange.ExchangeId;

            var existingExchange = GetAsync(
                exchangeId
                ).Result;

            if (existingExchange != null)
                _entities[exchangeId] = exchange;
        }

        private async Task DispatchEvents()
        {
            await _mediator.DispatchDomainEventsAsync(this._entities.Values);
        }

        private void Initialize()
        {
            var exchangeBittrex = new Exchange(0, 0.0025M, 0.0025M);
            var exchangeBinance = new Exchange(1, 0.0010M, 0.0010M);
            var exchangeKraken = new Exchange(2, 0.0026M, 0.0026M);
            var exchangeBitstamp = new Exchange(3, 0.0025M, 0.0025M);
            var exchangePoloniex = new Exchange(4, 0.0025M, 0.0025M);
            var exchangeKucoin = new Exchange(5, 0.0010M, 0.0010M);

            this.Add(exchangeBittrex);
            this.Add(exchangeBinance);
            this.Add(exchangeKraken);
            this.Add(exchangePoloniex);
            this.Add(exchangeBitstamp);
            this.Add(exchangeKucoin);
        }
    }
}
