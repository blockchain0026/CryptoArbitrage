using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.Events
{
    public class ExchangeAssetInfoUpdatedIntegrationEvent : IntegrationEvent
    {
        public int ExchangeId { get; private set; }
        public string AssetSimbol { get; private set; }
        public decimal TotalBalance { get; private set; }
        public decimal AvailableBalance { get; private set; }
        public decimal PendingBalance { get; private set; }


        public ExchangeAssetInfoUpdatedIntegrationEvent(int exchangeId, string assetSimbol, decimal totalBalance, decimal availableBalance, decimal pendingBalance)
        {
            ExchangeId = exchangeId;
            AssetSimbol = assetSimbol;
            TotalBalance = totalBalance;
            AvailableBalance = availableBalance;
            PendingBalance = pendingBalance;
        }
    }
}
