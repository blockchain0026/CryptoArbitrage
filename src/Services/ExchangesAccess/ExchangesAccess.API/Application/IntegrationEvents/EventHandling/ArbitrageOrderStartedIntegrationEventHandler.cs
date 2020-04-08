using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.EventBus.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using CryptoArbitrage.Services.ExchangesAccess.API.IntegrationEvents.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.EventHandling
{
    public class ArbitrageOrderStartedIntegrationEventHandler : IIntegrationEventHandler<ArbitrageOrderStartedIntegrationEvent>
    {
        private readonly IExchangeApiClient _exchangeApiClient;
        private readonly IEventBus _eventBus;

        public ArbitrageOrderStartedIntegrationEventHandler(IExchangeApiClient exchangeApiClient, IEventBus eventBus)
        {
            _exchangeApiClient = exchangeApiClient ?? throw new ArgumentNullException(nameof(exchangeApiClient));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task Handle(ArbitrageOrderStartedIntegrationEvent @event)
        {

            string exchangeId = @event.ExchangeId.ToString();
            string baseCurrency = @event.BaseCurrency;
            string quoteCurrency = @event.QuoteCurrency;

            //string market = command.BaseCurrency + command.QuoteCurrency;
            string quantity = @event.QuantityTotal.ToString();
            string price = @event.Price.ToString();

            OrderViewModel orderViewModel = null;


            if (@event.OrderType == "buy_limit")
            {

                var buyOrderViewModel = await _exchangeApiClient.PlaceBuyOrder(exchangeId, baseCurrency, quoteCurrency, quantity, price);
                if (buyOrderViewModel != null)
                    orderViewModel = new OrderViewModel { orderId = buyOrderViewModel.Result != null ? buyOrderViewModel.Result.orderId : String.Empty };
            }
            else if (@event.OrderType == "sell_limit")
            {
                var sellOrderViewModel = await _exchangeApiClient.PlaceSellOrder(exchangeId, baseCurrency, quoteCurrency, quantity, price);
                if (sellOrderViewModel != null)
                    orderViewModel = new OrderViewModel { orderId = sellOrderViewModel.Result != null ? sellOrderViewModel.Result.orderId : String.Empty };
            }




            /*this._eventBus.Publish(new ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent(
        @event.ArbitrageOrderId,
        @event.ExchangeId,
        @event.BaseCurrency,
        @event.QuoteCurrency,
        @event.QuantityTotal,
        @event.Price
        ));
            return;*/
            if (orderViewModel == null || String.IsNullOrEmpty(orderViewModel.orderId))
            {
                this._eventBus.Publish(new ArbitrageOrderSubmittedToExchangeFailedIntegrationEvent(
                    @event.ArbitrageOrderId,
                    @event.ExchangeId,
                    @event.BaseCurrency,
                    @event.QuoteCurrency,
                    @event.QuantityTotal,
                    @event.Price
                    ));
            }
            else
            {
                this._eventBus.Publish(new ArbitrageOrderSubmittedToExchangeSuccessIntegrationEvent(
                    @event.ArbitrageOrderId,
                    @event.ExchangeId,
                    orderViewModel.orderId,
                    @event.BaseCurrency,
                    @event.QuoteCurrency,
                    @event.QuantityTotal,
                    @event.Price
                    ));

                this._eventBus.Publish(new ExchangeOrderCreatedIntegrationEvent(
                    @event.ExchangeId,
                    orderViewModel.orderId,
                    @event.BaseCurrency,
                    @event.QuoteCurrency,
                    @event.QuantityTotal,
                    @event.Price
                    ));
            }

        }
    }
}
