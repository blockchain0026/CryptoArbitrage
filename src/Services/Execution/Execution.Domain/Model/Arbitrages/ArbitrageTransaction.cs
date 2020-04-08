using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public class ArbitrageTransaction : Entity
    {
        public string ArbitrageOrderId { get; private set; }

        //Cannot use value object because entity framework doesn't support it currently.
        public int ExchangeId { get; private set; }
        

        public OrderType OriginalOrderType { get; private set; }
        private int _originalOrderTypeId;

        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal Volume { get; private set; }
        public decimal CommisionPaid { get; private set; }

        public ArbitrageTransaction(string arbitrageOrderId, int exchangeId, int originalOrderTypeId, string baseCurrency, string quoteCurrency, decimal price, decimal volume, decimal commisionPaid)
        {
            ArbitrageOrderId = arbitrageOrderId ?? throw new ArgumentNullException(nameof(arbitrageOrderId));
            ExchangeId = exchangeId >= 0 ? exchangeId : throw new ArgumentOutOfRangeException(nameof(exchangeId));
            _originalOrderTypeId = originalOrderTypeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Volume = volume > 0 ? volume : throw new ArgumentOutOfRangeException(nameof(volume));
            CommisionPaid = commisionPaid > 0 ? commisionPaid : throw new ArgumentOutOfRangeException(nameof(commisionPaid));
        }
    }
}
