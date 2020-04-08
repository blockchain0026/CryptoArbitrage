using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.DomainEventHandlers.SimpleArbitrageOpened
{
    public class CreateBuyOrderWhenSimpleArbitrageOpenedDomainEventHandler
        : INotificationHandler<SimpleArbitrageOpenedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateBuyOrderWhenSimpleArbitrageOpenedDomainEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }


        public async Task Handle(SimpleArbitrageOpenedDomainEvent simpleArbitrageOpenedDomainEvent, CancellationToken cancellationToken)
        {
            
            var arbitrageBuyOrder = simpleArbitrageOpenedDomainEvent.SimpleArbitrage.BuyOrder;

            var buyOrderToCreate = new Order(
                arbitrageBuyOrder.ArbitrageOrderId,
                simpleArbitrageOpenedDomainEvent.SimpleArbitrage.ArbitrageId,
                arbitrageBuyOrder.ExchangeId,
                OrderType.BUY_LIMIT.Id,
                arbitrageBuyOrder.BaseCurrency,
                arbitrageBuyOrder.QuoteCurrency,
                arbitrageBuyOrder.Price,
                arbitrageBuyOrder.Quantity
                );

            this._orderRepository.Add(buyOrderToCreate);


            var success =this._orderRepository.UnitOfWork.SaveEntitiesAsync().Result;
            
            Debug.WriteLine("Handle Domain Event: SimpleArbitrageOpenedDomainEvent. \n" +
             "To Do: Create Buy Order. \n" +
             "Result:" + (success == true ? "success" : "false"));

            return;
        }
    }
}
