using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Utilitys
{
    public enum ExchangeApiMethodEnum
    {
        None = 0,
        GetMarket,
        GetOpenOrders,
        PlaceBuyOrder,
        PlaceSellOrder,
        CancelOrder,
        GetBalances
    }
}
