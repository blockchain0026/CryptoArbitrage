using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.DomainEventHandlers.OrderPartiallyFilled
{
    public class AddNewArbitrageTransactionWhenOrderPartiallyFilledDomainEventHandler : INotificationHandler<OrderPartiallyFilledDomainEvent>
    {
        private readonly ISimpleArbitrageRepository _simpleArbitrageRepository;
        private readonly IOrderRepository _orderRepository;
        public AddNewArbitrageTransactionWhenOrderPartiallyFilledDomainEventHandler(ISimpleArbitrageRepository simpleArbitrageRepository, IOrderRepository orderRepository)
        {
            this._simpleArbitrageRepository = simpleArbitrageRepository ?? throw new ArgumentNullException(nameof(simpleArbitrageRepository));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(OrderPartiallyFilledDomainEvent orderPartiallyFilledDomainEvent, CancellationToken cancellationToken)
        {
            var simpleArbitrage = await this._simpleArbitrageRepository.GetAsync(orderPartiallyFilledDomainEvent.ArbitrageId);
            var order = await this._orderRepository.GetAsync(orderPartiallyFilledDomainEvent.OrderId);
            if (simpleArbitrage == null)
                throw new KeyNotFoundException(nameof(orderPartiallyFilledDomainEvent.ArbitrageId));
            if (order == null)
                throw new KeyNotFoundException(nameof(orderPartiallyFilledDomainEvent.OrderId));


            simpleArbitrage.OrderFilled(order);
            this._simpleArbitrageRepository.Update(simpleArbitrage);

            await this._simpleArbitrageRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
