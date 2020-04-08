using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.DomainEventHandlers.OrderRejected
{

    public class OrderRejectedDomainEventHandler : INotificationHandler<OrderRejectedDomainEvent>
    {
        private readonly ISimpleArbitrageRepository _simpleArbitrageRepository;
        private readonly IOrderRepository _orderRepository;
        public OrderRejectedDomainEventHandler(ISimpleArbitrageRepository simpleArbitrageRepository, IOrderRepository orderRepository)
        {
            this._simpleArbitrageRepository = simpleArbitrageRepository ?? throw new ArgumentNullException(nameof(simpleArbitrageRepository));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(OrderRejectedDomainEvent orderCanceledDomainEvent, CancellationToken cancellationToken)
        {
            var simpleArbitrage = await this._simpleArbitrageRepository.GetAsync(orderCanceledDomainEvent.ArbitrageId);
            var order = await this._orderRepository.GetAsync(orderCanceledDomainEvent.OrderId);
            if (simpleArbitrage == null)
                throw new KeyNotFoundException(nameof(orderCanceledDomainEvent.ArbitrageId));
            if (order == null)
                throw new KeyNotFoundException(nameof(orderCanceledDomainEvent.OrderId));

            Order buyOrder = null;
            Order sellOrder = null;

            if (order.OrderType.Id == OrderType.BUY_LIMIT.Id)
            {
                buyOrder = order;
                sellOrder = await this._orderRepository.GetAsync(simpleArbitrage.SellOrder.ArbitrageOrderId);
            }
            else if (order.OrderType.Id == OrderType.SELL_LIMIT.Id)
            {
                sellOrder = order;
                buyOrder = await this._orderRepository.GetAsync(simpleArbitrage.BuyOrder.ArbitrageOrderId);
            }

            if (buyOrder != null && sellOrder != null)
            {
                try
                {
                    simpleArbitrage.Close(buyOrder, sellOrder);
                    this._simpleArbitrageRepository.Update(simpleArbitrage);
                    await this._simpleArbitrageRepository.UnitOfWork.SaveEntitiesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Trying to close simple arbirtage false.");
                    Console.WriteLine("Arbitrage Id: " + simpleArbitrage.ArbitrageId);
                    Console.WriteLine("Error Message: " + ex.Message);
                }
            }
        }
    }
}
