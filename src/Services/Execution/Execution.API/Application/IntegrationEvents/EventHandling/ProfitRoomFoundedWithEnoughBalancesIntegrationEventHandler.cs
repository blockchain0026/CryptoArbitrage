using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.API.Infrastructure;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling
{
    public class ProfitRoomFoundedWithEnoughBalancesIntegrationEventHandler : IIntegrationEventHandler<ProfitRoomFoundedWithEnoughBalancesIntegrationEvent>
    {
        private readonly ISimpleArbitrageRepository _simpleArbitrageRepository;
        private readonly IStopLossSettingRepository _stopLossSettingsRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IExchangeApiRequestFrequenciesControlService _exchangeApiRequestFrequenciesControlService;
        public ProfitRoomFoundedWithEnoughBalancesIntegrationEventHandler(
            ISimpleArbitrageRepository simpleArbitrageRepository, IStopLossSettingRepository stopLossSettingRepository, IOrderRepository orderRepository,
            IExchangeApiRequestFrequenciesControlService exchangeApiRequestFrequenciesControlService)
        {
            this._simpleArbitrageRepository = simpleArbitrageRepository ?? throw new ArgumentNullException(nameof(simpleArbitrageRepository));
            this._stopLossSettingsRepository = stopLossSettingRepository ?? throw new ArgumentNullException(nameof(stopLossSettingRepository));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._exchangeApiRequestFrequenciesControlService = exchangeApiRequestFrequenciesControlService ?? throw new ArgumentNullException(nameof(exchangeApiRequestFrequenciesControlService));
        }

        public async Task Handle(ProfitRoomFoundedWithEnoughBalancesIntegrationEvent @event)
        {
            if (@event.BuyOrderAmounts < 0.0001M || @event.SellOrderAmounts < 0.0001M)
            {
                return;
            }

            lock (_exchangeApiRequestFrequenciesControlService)
            {
               
                if (!_exchangeApiRequestFrequenciesControlService.IsRequestAllow(@event.BuyFrom) || !_exchangeApiRequestFrequenciesControlService.IsRequestAllow(@event.SellTo))
                {
                    return;
                }
                else
                {
                    this._exchangeApiRequestFrequenciesControlService.UpdateRequestsTime(@event.BuyFrom);
                    this._exchangeApiRequestFrequenciesControlService.UpdateRequestsTime(@event.SellTo);
                }
            }
   
         
            var exchangeBuyFrom = new Exchange(@event.BuyFrom);
            var exchangeSellTo = new Exchange(@event.SellTo);

            /*var buyStopLoss = await _stopLossSettingsRepository.GetByExchangeAsync(exchangeBuyFrom);
            var sellStopLoss = await this._stopLossSettingsRepository.GetByExchangeAsync(exchangeSellTo);*/
            var buyStopLoss = _stopLossSettingsRepository.GetByExchangeAsync(exchangeBuyFrom).Result;
            var sellStopLoss = _stopLossSettingsRepository.GetByExchangeAsync(exchangeSellTo).Result;

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



            //Prevent too frequently request.
            /*this._exchangeApiRequestFrequenciesControlService.UpdateRequestsTime(exchangeBuyFrom.ExchangeId);
            this._exchangeApiRequestFrequenciesControlService.UpdateRequestsTime(exchangeSellTo.ExchangeId);*/

            Debug.WriteLine("Handle Event: ProfitRoomFoundedWithEnoughBalancesIntegrationEvent. \n" +
                "To Do: Create A New Simple Arbitrage Into Database. \n" +
                "Result:" + (success == true ? "success" : "false"));



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
