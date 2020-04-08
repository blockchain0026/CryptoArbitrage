using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.IntegrationEvents.EventHandling
{
    public class ProfitRoomFoundedIntegrationEventHandler : IIntegrationEventHandler<ProfitRoomFoundedIntegrationEvent>
    {
        private readonly ISimpleArbitrageRepository _simpleArbitrageRepository;
        private readonly IStopLossSettingRepository _stopLossSettingsRepository;
        private readonly IOrderRepository _orderRepository;
        public ProfitRoomFoundedIntegrationEventHandler(ISimpleArbitrageRepository simpleArbitrageRepository, IStopLossSettingRepository stopLossSettingRepository, IOrderRepository orderRepository)
        {
            this._simpleArbitrageRepository = simpleArbitrageRepository ?? throw new ArgumentNullException(nameof(simpleArbitrageRepository));
            this._stopLossSettingsRepository = stopLossSettingRepository ?? throw new ArgumentNullException(nameof(stopLossSettingRepository));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(ProfitRoomFoundedIntegrationEvent @event)
        {
            return;
            var exchangeBuyFrom = new Exchange(@event.BuyFrom);
            var exchangeSellTo = new Exchange(@event.SellTo);

            /*var buyStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeBuyFrom);
            var sellStopLoss = await this._stopLossSettingsRepository.GetByExchangeAsync(exchangeSellTo);*/
            var buyStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeBuyFrom);
            var sellStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeSellTo);

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
                "To Do: Create A New Simple Arbitrage Into Database. \n" +
                "Result:" + (success == true ? "success" : "false"));
            /*var exchangeBuyFrom = new Exchange(@event.BuyFrom);
            var exchangeSellTo = new Exchange(@event.SellTo);

            var buyStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeBuyFrom);
            var sellStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeSellTo);

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

            var success = await this._simpleArbitrageRepository.UnitOfWork.SaveEntitiesAsync();*/
            /*var success = true;

            Debug.WriteLine("Handle Event: ProfitRoomFoundedIntegrationEvent. \n" +
                "To Do: Nothing. \n" +
                "Result:" + (success == true ? "success" : "false"));*/

            /*var arbitrageSellOrder = simpleArbitrage.SellOrder;

            var sellOrderToCreate = new Order(
                arbitrageSellOrder.ArbitrageOrderId,
                arbitrageSellOrder.ExchangeId,
                OrderType.SELL_LIMIT.Id,
                arbitrageSellOrder.BaseCurrency,
                arbitrageSellOrder.QuoteCurrency,
                arbitrageSellOrder.Price,
                arbitrageSellOrder.Quantity
                );


            this._orderRepository.Add(sellOrderToCreate);


            success = await this._orderRepository.UnitOfWork.SaveEntitiesAsync();
            Debug.WriteLine("Handle Event: SimpleArbitrageOpenedDomainEvent. \n" +
                "Result:" + (success == true ? "success" : "false"));*/

        }
    }
}
