using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeOrderFullExecutedIntegrationEventHandler:IIntegrationEventHandler<ExchangeOrderFullExecutedIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;
        public ExchangeOrderFullExecutedIntegrationEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(ExchangeOrderFullExecutedIntegrationEvent @event)
        {
            var order = await _orderRepository.GetByExchangeOrderIdAsync(@event.ExchangeOrderId);
            if(order!=null)
            {
                order.TryMarkAsFullFilled();
            }
            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
