using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Orders
{
    public interface IExchangeService
    {
        void SubmitOrder(Exchange exchange, OrderType orderType, string baseCurrency, string quoteCurrency, decimal price, decimal quantity);
        bool CancelOrder(Exchange exchange, string exchangeOrderId, string baseCurrency, string quoteCurrency);
    }
}
