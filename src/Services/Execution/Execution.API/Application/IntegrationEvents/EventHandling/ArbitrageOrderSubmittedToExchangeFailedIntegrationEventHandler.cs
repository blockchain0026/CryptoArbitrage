using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling
{
    public class ArbitrageOrderSubmittedToExchangeFailedIntegrationEventHandler : IIntegrationEventHandler<ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public ArbitrageOrderSubmittedToExchangeFailedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }


        public async Task Handle(ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent @event)
        {
            var order = await this._orderRepository.GetAsync(@event.ArbitrageOrderId);

            if (order != null)
            {
                order.ExchangeOrderRejected(String.Empty);
            }
            else
            {
                throw new KeyNotFoundException(nameof(order));
            }

            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
