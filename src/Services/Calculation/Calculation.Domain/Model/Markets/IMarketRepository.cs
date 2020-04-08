using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Markets
{
    public interface IMarketRepository : IRepository<Market>
    {
        Market Add(Market market);

        Task Update(Market market);

        Task<Market> GetAsync(MarketId marketId);
    }
}
