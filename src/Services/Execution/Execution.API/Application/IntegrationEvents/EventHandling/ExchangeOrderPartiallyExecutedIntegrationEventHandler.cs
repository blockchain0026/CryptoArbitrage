using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeOrderPartiallyExecutedIntegrationEventHandler : IIntegrationEventHandler<ExchangeOrderPartiallyExecutedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public ExchangeOrderPartiallyExecutedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(ExchangeOrderPartiallyExecutedIntegrationEvent @event)
        {
            var order = await this._orderRepository.GetByExchangeOrderIdAsync(@event.ExchangeOrderId);
            if (order != null)
            {
                order.ExchangeOrderFilled(@event.QuantityExecuted, @event.CommisionPaid, @event.CreationDate);

                _orderRepository.Update(order);
                await _orderRepository.UnitOfWork.SaveEntitiesAsync();
            }
        }
    }
}
