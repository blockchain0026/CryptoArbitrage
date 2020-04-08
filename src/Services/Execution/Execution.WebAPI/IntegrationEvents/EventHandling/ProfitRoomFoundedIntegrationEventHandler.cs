using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using CryptoArbitrage.Services.Execution.WebAPI.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.WebAPI.IntegrationEvents.EventHandling
{
    public class ProfitRoomFoundedIntegrationEventHandler : IIntegrationEventHandler<ProfitRoomFoundedIntegrationEvent>
    {
        private readonly ISimpleArbitrageRepository _simpleArbitrageRepository;
        private readonly IStopLossSettingRepository _stopLossSettingsRepository;
        public ProfitRoomFoundedIntegrationEventHandler(ISimpleArbitrageRepository simpleArbitrageRepository, IStopLossSettingRepository stopLossSettingRepository)
        {
            this._simpleArbitrageRepository = simpleArbitrageRepository ?? throw new ArgumentNullException(nameof(simpleArbitrageRepository));
            this._stopLossSettingsRepository = stopLossSettingRepository ?? throw new ArgumentNullException(nameof(stopLossSettingRepository));
        }

        public async Task Handle(ProfitRoomFoundedIntegrationEvent @event)
        {
            return;
            var exchangeBuyFrom = new Exchange(@event.BuyFrom);
            var exchangeSellTo = new Exchange(@event.SellTo);

            /*var buyStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeBuyFrom);
            var sellStopLoss = await this._stopLossSettingsRepository.GetByExchangeAsync(exchangeSellTo);*/
            var buyStopLoss = await _stopLossSettingsRepository.GetByIdAsync("0");
            var sellStopLoss = await _stopLossSettingsRepository.GetByIdAsync("1");

            var simpleArbitrage = SimpleArbitrage.CreateFrom(
                @event.EstimatedProfits,
                @event.BuyOrderBaseCurrency,
                @event.SellOrderQuoteCurrency,
                @event.BuyFrom,
                @event.BuyOrderPrice,
                @event.BuyOrderAmounts,
                @event.SellTo,
                @event.SellOrderPrice,
                @event.SellOrderAmounts,
                buyStopLoss ?? new StopLossSetting(exchangeBuyFrom),
                sellStopLoss ?? new StopLossSetting(exchangeSellTo)
                );

            this._simpleArbitrageRepository.Add(simpleArbitrage);

            var success = await this._simpleArbitrageRepository.UnitOfWork.SaveEntitiesAsync();

            Debug.WriteLine("Handle Event: ProfitRoomFoundedIntegrationEvent. \n" +
                "Result:" + (success == true ? "success" : "false"));
        }
    }
}
