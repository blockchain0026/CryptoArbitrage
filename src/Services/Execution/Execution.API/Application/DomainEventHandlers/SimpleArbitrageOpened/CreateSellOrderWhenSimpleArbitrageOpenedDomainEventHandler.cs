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
    public class CreateSellOrderWhenSimpleArbitrageOpenedDomainEventHandler
       : INotificationHandler<SimpleArbitrageOpenedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateSellOrderWhenSimpleArbitrageOpenedDomainEventHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }


        public async Task Handle(SimpleArbitrageOpenedDomainEvent simpleArbitrageOpenedDomainEvent, CancellationToken cancellationToken)
        {
            
            //await Task.Run(async () => { });
            var arbitrageSellOrder = simpleArbitrageOpenedDomainEvent.SimpleArbitrage.SellOrder;

            var sellOrderToCreate = new Order(
                arbitrageSellOrder.ArbitrageOrderId,
                simpleArbitrageOpenedDomainEvent.SimpleArbitrage.ArbitrageId,
                arbitrageSellOrder.ExchangeId,
                OrderType.SELL_LIMIT.Id,
                arbitrageSellOrder.BaseCurrency,
                arbitrageSellOrder.QuoteCurrency,
                arbitrageSellOrder.Price,
                arbitrageSellOrder.Quantity
                );


            this._orderRepository.Add(sellOrderToCreate);

            var success = this._orderRepository.UnitOfWork.SaveEntitiesAsync().Result;

            Debug.WriteLine("Handle Domain Event: SimpleArbitrageOpenedDomainEvent. \n" +
                "To Do: Create Sell Order. \n" +
                "Result: " + (success == true ? "success" : "false"));

            return;
        }
    }
}
