using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.StopLoss
{
    public class StopLossSetting : Entity, IAggregateRoot
    {
        #region Private Fields
        private readonly List<SlipPrice> _slipPrices;
        #endregion


        #region Properties
        public Exchange Exchange { get; private set; }
        public IReadOnlyCollection<SlipPrice> SlipPrices => _slipPrices;
        #endregion


        protected StopLossSetting()
        {
            this._slipPrices = new List<SlipPrice>();
        }
        public StopLossSetting(Exchange exchange) : this()
        {
            this.Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
        }

        public decimal GetSlipPrice(decimal originalPrice, string baseCurrency, string quoteCurrency)
        {
            if (String.IsNullOrEmpty(baseCurrency))
                throw new ArgumentNullException(nameof(baseCurrency));
            if (String.IsNullOrEmpty(quoteCurrency))
                throw new ArgumentNullException(nameof(quoteCurrency));

            var slipPrice = _slipPrices.Where(s => s.BaseCurrency == baseCurrency && s.QuoteCurrency == quoteCurrency).SingleOrDefault();

            if (slipPrice == null)
            {
                return originalPrice * SlipPrice.DefaultSlipPercents;
            }
            else
            {
                return originalPrice * slipPrice.SlipPercents;
            }
        }
    }
}
