using CryptoArbitrage.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.Events
{
    public class ExchangeMultiAssetsUpdatedIntegrationEvent : IntegrationEvent
    {
        public int ExchangeId { get; private set; }

        public ExchangeMultiAssetsIntegrationModel ExchangeAssets { get; private set; }


        public ExchangeMultiAssetsUpdatedIntegrationEvent(int exchangeId, ExchangeMultiAssetsIntegrationModel assets)
        {
            ExchangeId = exchangeId;
            ExchangeAssets = assets;
        }


    }
    public class ExchangeAssetIntegationModel
    {
        public string assetsymbol { get; set; }
        public decimal totalbalance { get; set; }
        public decimal availablebalance { get; set; }
        public decimal pendingbalance { get; set; }
    }

    public class ExchangeMultiAssetsIntegrationModel
    {
        public List<ExchangeAssetIntegationModel> assets { get; set; }
    }
}
