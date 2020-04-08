using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeOrderCanceledIntegrationEventHandler : IIntegrationEventHandler<ExchangeOrderCanceledIntegrationEvent>
    {
        private readonly IOrderRepository _orderRepository;
        public ExchangeOrderCanceledIntegrationEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(ExchangeOrderCanceledIntegrationEvent @event)
        {
            var order = await _orderRepository.GetByExchangeOrderIdAsync(@event.ExchangeOrderId);
            DateTime dateCanceled;
            if (order != null)
            {
                var isParseSuccess = DateTime.TryParse(@event.DateCanceled, out dateCanceled);
                if (!isParseSuccess)
                    dateCanceled = @event.CreationDate;

                order.ExchangeOrderCanceled(@event.ExchangeOrderId, dateCanceled);
            }
            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
