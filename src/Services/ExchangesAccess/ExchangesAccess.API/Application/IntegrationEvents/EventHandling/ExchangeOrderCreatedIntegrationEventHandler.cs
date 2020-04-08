using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.EventHandling
{
    public class ExchangeOrderCreatedIntegrationEventHandler : IIntegrationEventHandler<ExchangeOrderCreatedIntegrationEvent>
    {
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IEventBus _eventBus;

        public ExchangeOrderCreatedIntegrationEventHandler(IExchangeApiClient exchangeApiClient, IEventBus eventBus)
        {
            _exchangeApiClient = exchangeApiClient ?? throw new ArgumentNullException(nameof(exchangeApiClient));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task Handle(ExchangeOrderCreatedIntegrationEvent @event)
        {
            var orderClosed = false;
            var orderCanceled = false;
            string closeTime = String.Empty;
            string canceledTime = String.Empty;

            decimal quantityAlreadyExecuted = 0;
            decimal commisionAlreadyPaid = 0;

            await Task.Run(async () =>
            {
                while (!orderClosed && !orderCanceled)
                {
                    var openOrderViewModel = await _exchangeApiClient.GetOpenOrder(@event.ExchangeId.ToString(), @event.BaseCurrency, @event.QuoteCurrency);

                    OrderViewModel matchingOrder = null;

                    foreach (var openOrder in openOrderViewModel.Result.orders)
                    {
                        if (openOrder.orderId == @event.ExchangeOrderId)
                        {
                            matchingOrder = openOrder;
                        }
                    }

                    if (matchingOrder == null)
                    {
                        orderClosed = true;
                        closeTime = DateTime.UtcNow.ToLongDateString();
                    }

                    if (matchingOrder != null)
                    {
                        var quantity = Decimal.Parse(matchingOrder.quantityExecuted, NumberStyles.Float);
                        var paidFee = Decimal.Parse(matchingOrder.commissionPaid, NumberStyles.Float);


                        //If quantity executed and total quantity are equal.
                        if (quantity == Decimal.Parse(matchingOrder.quantityTotal, NumberStyles.Float))
                        {

                            this._eventBus.Publish(new ExchangeOrderPartiallyExecutedIntegrationEvent(
                              @event.ExchangeId,
                              @event.ExchangeOrderId,
                              @event.BaseCurrency,
                              @event.QuoteCurrency,
                              quantity - quantityAlreadyExecuted,
                              @event.Price,
                              paidFee - commisionAlreadyPaid
                              ));

                            orderClosed = true;
                        }
                        // If order are canceled
                        else if (matchingOrder.canceled == "true")
                        {
                            if (quantity > quantityAlreadyExecuted)
                            {
                                this._eventBus.Publish(new ExchangeOrderPartiallyExecutedIntegrationEvent(
                                @event.ExchangeId,
                                @event.ExchangeOrderId,
                                @event.BaseCurrency,
                                @event.QuoteCurrency,
                                quantity - quantityAlreadyExecuted,
                                @event.Price,
                                paidFee - commisionAlreadyPaid
                                ));

                                quantityAlreadyExecuted = quantity;
                                commisionAlreadyPaid = paidFee;
                            }

                            orderCanceled = true;
                        }
                        else
                        {
                            if (quantity > quantityAlreadyExecuted)
                            {
                                this._eventBus.Publish(new ExchangeOrderPartiallyExecutedIntegrationEvent(
                                    @event.ExchangeId,
                                    @event.ExchangeOrderId,
                                    @event.BaseCurrency,
                                    @event.QuoteCurrency,
                                    quantity - quantityAlreadyExecuted,
                                    @event.Price,
                                    paidFee - commisionAlreadyPaid
                                    ));

                                quantityAlreadyExecuted = quantity;
                                commisionAlreadyPaid = paidFee;
                            }
                        }
                    }

                    //Keep detecting until order closed.
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
            });

            if (orderClosed)
            {
                this._eventBus.Publish(new ExchangeOrderFullExecutedIntegrationEvent(
                    @event.ExchangeId,
                    @event.ExchangeOrderId,
                    @event.BaseCurrency,
                    @event.QuoteCurrency,
                    @event.Quantity,
                    @event.Price,
                    commisionAlreadyPaid,
                    closeTime
                    ));
            }
            else if (orderCanceled)
            {
                this._eventBus.Publish(new ExchangeOrderCanceledIntegrationEvent(
                    @event.ExchangeId,
                    @event.ExchangeOrderId,
                    @event.BaseCurrency,
                    @event.QuoteCurrency,
                    quantityAlreadyExecuted,
                    @event.Price,
                    commisionAlreadyPaid,
                    canceledTime
                    ));
            }

        }
    }
}
