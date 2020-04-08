using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public interface IExchangeMarketData
    {
        OrderBookViewModel GetMarketData(string exchangeId, string baseCurrency, string quoteCurrency);
        Task InitializeMarketData(string exchangeId, string baseCurrency, string quoteCurrency);
        Task WriteMarketData(string exchangeId, string baseCurrency, string quoteCurrency, string data);
    }
}
