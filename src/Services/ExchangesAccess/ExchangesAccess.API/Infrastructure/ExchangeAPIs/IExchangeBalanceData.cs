using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public interface IExchangeBalanceData
    {
        Task WriteBalanceData(string exchangeId, string symbol, decimal totalBalance, decimal availableBalance, decimal pendingBalance);
    }
}