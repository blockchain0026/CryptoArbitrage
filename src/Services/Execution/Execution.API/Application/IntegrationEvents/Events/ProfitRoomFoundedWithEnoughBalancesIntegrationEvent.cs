using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events
{
    public class ProfitRoomFoundedWithEnoughBalancesIntegrationEvent : IntegrationEvent
    {
        public int BuyFrom { get; private set; }
        public string BuyOrderBaseCurrency { get; private set; }
        public string BuyOrderQuoteCurrency { get; private set; }
        public decimal BuyOrderPrice { get; private set; }
        public decimal BuyOrderAmounts { get; private set; }

        public int SellTo { get; private set; }
        public string SellOrderBaseCurrency { get; private set; }
        public string SellOrderQuoteCurrency { get; private set; }
        public decimal SellOrderPrice { get; private set; }
        public decimal SellOrderAmounts { get; private set; }
        public decimal EstimatedProfits { get; private set; }
        public ProfitRoomFoundedWithEnoughBalancesIntegrationEvent
          (int buyFrom, string buyOrderBaseCurrency, string buyOrderQuoteCurrency, decimal buyOrderPrice, decimal buyOrderAmounts,
          int sellTo, string sellOrderBaseCurrency, string sellOrderQuoteCurrency, decimal sellOrderPrice, decimal sellOrderAmounts,
          decimal estimatedProfits)
        {
            BuyFrom = buyFrom;
            BuyOrderBaseCurrency = buyOrderBaseCurrency;
            BuyOrderQuoteCurrency = buyOrderQuoteCurrency;
            BuyOrderPrice = buyOrderPrice;
            BuyOrderAmounts = buyOrderAmounts;
            SellTo = sellTo;
            SellOrderBaseCurrency = sellOrderBaseCurrency;
            SellOrderQuoteCurrency = sellOrderQuoteCurrency;
            SellOrderPrice = sellOrderPrice;
            SellOrderAmounts = sellOrderAmounts;
            EstimatedProfits = estimatedProfits;
        }
    }
}
