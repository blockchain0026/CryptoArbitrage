using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public interface IExchangeApiClient
    {
        Task<ExchangeApiResponse<CancelOrderViewModel>> CancelOrder(string exchangeId, string orderId, string baseCurrency = null, string quoteCurrency = null);
        Task<ExchangeApiResponse<BalanceViewModel>> GetBalances(string exchangeId, string baseCurrency, string quoteCurrency);
        Task<ExchangeApiResponse<GetOpenOrderViewModel>> GetOpenOrder(string exchangeId, string baseCurrency, string quoteCurrency);
        Task<ExchangeApiResponse<OrderBookViewModel>> GetOrderBook(string exchangeId, string baseCurrency, string quoteCurrency);
        Task<ExchangeApiResponse<PlaceBuyOrderViewModel>> PlaceBuyOrder(string exchangeId, string baseCurrency, string quoteCurrency, string quantity, string price);
        Task<ExchangeApiResponse<PlaceSellOrderViewModel>> PlaceSellOrder(string exchangeId, string baseCurrency, string quoteCurrency, string quantity, string price);
    }
}
