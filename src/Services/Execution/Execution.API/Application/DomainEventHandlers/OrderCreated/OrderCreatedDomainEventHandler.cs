using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.DomainEventHandlers.OrderCreated
{
    public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly ISimpleArbitrageRepository _simpleArbitrageRepository;
        private readonly IOrderRepository _orderRepository;
        public OrderCreatedDomainEventHandler(ISimpleArbitrageRepository simpleArbitrageRepository, IOrderRepository orderRepository)
        {
            this._simpleArbitrageRepository = simpleArbitrageRepository ?? throw new ArgumentNullException(nameof(simpleArbitrageRepository));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(OrderCreatedDomainEvent orderCreatedDomainEvent, CancellationToken cancellationToken)
        {
            
            var simpleArbitrage = await this._simpleArbitrageRepository.GetAsync(orderCreatedDomainEvent.ArbitrageId);
            var order = await this._orderRepository.GetAsync(orderCreatedDomainEvent.OrderId);
            if (simpleArbitrage == null)
                throw new KeyNotFoundException(nameof(orderCreatedDomainEvent.ArbitrageId));
            if (order == null)
                throw new KeyNotFoundException(nameof(orderCreatedDomainEvent.OrderId));

            simpleArbitrage.OrderCreated(order);

            await _simpleArbitrageRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
