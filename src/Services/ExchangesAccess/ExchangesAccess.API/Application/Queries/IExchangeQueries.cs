using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries
{
    public interface IExchangeQueries
    {
        Task<ExchangeViewModel> GetExchangeAsync(int id);
        Task<OrderBookViewModel> GetMarketAsync(int exchangeId, string baseCurrency, string quoteCurrency);
    }
}
