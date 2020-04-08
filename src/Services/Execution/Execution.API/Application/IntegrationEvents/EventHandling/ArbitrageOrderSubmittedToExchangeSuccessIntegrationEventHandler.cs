using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling
{
    public class ArbitrageOrderSubmittedToExchangeSuccessIntegrationEventHandler : IIntegrationEventHandler<ArbitrageOrderSubmittedToExchangeSuccessIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public ArbitrageOrderSubmittedToExchangeSuccessIntegrationEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }


        public async Task Handle(ArbitrageOrderSubmittedToExchangeSuccessIntegrationEvent @event)
        {
            var order = await this._orderRepository.GetAsync(@event.ArbitrageOrderId);

            if (order != null)
            {
                order.ExchangeOrderCreated(@event.ExchangeOrderId,@event.CreationDate);
            }
            else
            {
                throw new KeyNotFoundException(nameof(order));
            }

            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
