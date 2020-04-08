using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents;
using CryptoArbitrage.Services.Execution.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Application.DomainEventHandlers.OrderStarted
{
    public class SubmitOrderToExchangeWhenOrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        //private readonly IEventBus _eventBus;
        private readonly IOrderRepository _orderRepository;
        private readonly IExecutionIntegrationEventService _executionIntegrationEventService;
        public SubmitOrderToExchangeWhenOrderStartedDomainEventHandler(IExecutionIntegrationEventService executionIntegrationEventService, IOrderRepository orderRepository)
        {
            this._executionIntegrationEventService = executionIntegrationEventService ?? throw new ArgumentNullException(nameof(executionIntegrationEventService));
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(OrderStartedDomainEvent orderStartedDomainEvent, CancellationToken cancellationToken)
        {

            
            try
            {

                var order = this._orderRepository.GetAsync(orderStartedDomainEvent.OrderId).Result;
                if (order == null)
                {
                    //return Task.FromResult(false);
                    return;
                }
                order.ExchangeOrderSubmitted();
                this._orderRepository.Update(order);
                await this._orderRepository.UnitOfWork.SaveEntitiesAsync();

                await this._executionIntegrationEventService.PublishThroughEventBusAsync(new ArbitrageOrderStartedIntegrationEvent(
                         orderStartedDomainEvent.OrderId,
                         orderStartedDomainEvent.ArbitrageId,
                         orderStartedDomainEvent.Exchange.ExchangeId,
                         orderStartedDomainEvent.OrderType.Name,
                         orderStartedDomainEvent.BaseCurrency,
                         orderStartedDomainEvent.QuoteCurrency,
                         orderStartedDomainEvent.Price,
                         orderStartedDomainEvent.QuantityTotal
                         ));

                Debug.WriteLine(
            "Handle Domain Event: OrderStartedDomainEvent. \n" +
            "To Do: Change order status and publish AbitrageOrderStartedIntegrationEvent. \n" +
            "Result: Success." );
                //return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Handle Domain Event: OrderStartedDomainEvent. \n"+
                    "To Do: Change order status and publish AbitrageOrderStartedIntegrationEvent. \n"+
                    "Result: Failed. \n"+
                    "Error Message: " +ex.Message);
                //return Task.FromResult(false);
            }
         
        }
    }
}

