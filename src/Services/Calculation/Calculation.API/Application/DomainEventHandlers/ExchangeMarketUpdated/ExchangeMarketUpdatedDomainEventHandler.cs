using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Calculation.API.Application.IntegrationEvents.Events;
using CryptoArbitrage.Services.Calculation.API.Infrastructure;
using CryptoArbitrage.Services.Calculation.API.IntegrationEvents.Events;
using CryptoArbitrage.Services.Calculation.Domain.Events;
using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using CryptoArbitrage.Services.Calculation.Domain.Model.Profits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Application.DomainEventHandlers.ExchangeMarketUpdated
{
    public class ExchangeMarketUpdatedDomainEventHandler : INotificationHandler<ExchangeMarketUpdatedDomainEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly ITestLogger _testLogger;
        private readonly IExchangeRepository _exchangeRepository;
        public ExchangeMarketUpdatedDomainEventHandler(IEventBus eventBus, ITestLogger testLogger, IExchangeRepository exchangeRepository)
        {
            _eventBus = eventBus;
            _testLogger = testLogger ?? throw new ArgumentNullException(nameof(testLogger));
            _exchangeRepository = exchangeRepository ?? throw new ArgumentException(nameof(exchangeRepository));

            /*_profitCalculator = profitCalculator ?? throw new ArgumentNullException(nameof(profitCalculator));*/
            /*_profitCalculator = new ProfitCalculator(
                marketRepository ?? throw new ArgumentNullException(nameof(marketRepository)),
                exchangeRepository ?? throw new ArgumentNullException(nameof(exchangeRepository))
                );*/
        }

        public async Task Handle(ExchangeMarketUpdatedDomainEvent exchangeMarketUpdatedDomainEvent, CancellationToken cancellationToken)
        {
            /*_logger.CreateLogger(nameof(ExchangeMarketUpdatedDomainEvent))
             .LogTrace($"Order with Id: {orderShippedDomainEvent.Order.Id} has been successfully updated with " +
                       $"a status order id: {OrderStatus.Shipped.Id}");*/
      
            try
            {
                var marketId = exchangeMarketUpdatedDomainEvent.Market.MarketId;

                IEnumerable<SimpleArbitrage> simpleArbitrages;
                var isFound = await ProfitCalculator.FindProfits(out simpleArbitrages, marketId.BaseCurrency, marketId.QuoteCurrency, exchangeMarketUpdatedDomainEvent.Market,exchangeMarketUpdatedDomainEvent.ExchangeId);
                if (isFound)
                {
                    SimpleArbitrage profitableWithEnoughBalances = null;

                    foreach (var simpleArbitrage in simpleArbitrages)
                    {
                        this._eventBus.Publish(new ProfitRoomFoundedIntegrationEvent(
                             simpleArbitrage.BuyOrder.ExchangeId,
                             simpleArbitrage.BuyOrder.BaseCurrency,
                             simpleArbitrage.BuyOrder.QuoteCurrency,
                             simpleArbitrage.BuyOrder.Price,
                             simpleArbitrage.BuyOrder.Quantity,
                             simpleArbitrage.SellOrder.ExchangeId,
                             simpleArbitrage.SellOrder.BaseCurrency,
                             simpleArbitrage.SellOrder.QuoteCurrency,
                             simpleArbitrage.SellOrder.Price,
                             simpleArbitrage.SellOrder.Quantity,
                             simpleArbitrage.EstimatedProfits
                             ));

                        if (profitableWithEnoughBalances == null)
                        {

                            var consideration = simpleArbitrage.TakeBalanceIntoConsideration(
                                this._exchangeRepository.GetAsync(simpleArbitrage.BuyOrder.ExchangeId).Result.GetAssetBalance(simpleArbitrage.BuyOrder.QuoteCurrency),
                                this._exchangeRepository.GetAsync(simpleArbitrage.SellOrder.ExchangeId).Result.GetAssetBalance(simpleArbitrage.SellOrder.BaseCurrency)

                                );

                            if (consideration.EstimatedProfits > 0)
                                profitableWithEnoughBalances = consideration;
                        }
                    }

                    if (profitableWithEnoughBalances != null)
                    {
                        this._eventBus.Publish(new ProfitRoomFoundedWithEnoughBalancesIntegrationEvent(
                                  profitableWithEnoughBalances.BuyOrder.ExchangeId,
                                  profitableWithEnoughBalances.BuyOrder.BaseCurrency,
                                  profitableWithEnoughBalances.BuyOrder.QuoteCurrency,
                                  profitableWithEnoughBalances.BuyOrder.Price,
                                  profitableWithEnoughBalances.BuyOrder.Quantity,
                                  profitableWithEnoughBalances.SellOrder.ExchangeId,
                                  profitableWithEnoughBalances.SellOrder.BaseCurrency,
                                  profitableWithEnoughBalances.SellOrder.QuoteCurrency,
                                  profitableWithEnoughBalances.SellOrder.Price,
                                  profitableWithEnoughBalances.SellOrder.Quantity,
                                  profitableWithEnoughBalances.EstimatedProfits
                                  ));
                    }
                }
                else
                {
                    /*string log = "Last calculation result:" + "\n"
                               + "Time:" + DateTime.UtcNow.ToShortTimeString() + "\n"
                               + "Market:" + simpleArbitrage.BuyOrder.BaseCurrency + simpleArbitrage.BuyOrder.QuoteCurrency + "\n"
                               + "Buy From:" + "ExchangeId => " + simpleArbitrage.BuyOrder.ExchangeId + "\n"
                               + "Buy Price:" + simpleArbitrage.BuyOrder.Price + "\n"
                               + "Buy Amount:" + simpleArbitrage.BuyOrder.Quantity + "\n"
                               + "Sell To:" + "ExchangeId => " + simpleArbitrage.SellOrder.ExchangeId + "\n"
                               + "Sell Price:" + simpleArbitrage.SellOrder.Price + "\n"
                               + "Sell Amount:" + simpleArbitrage.SellOrder.Quantity + "\n"
                               + "Estimated Profits" + simpleArbitrage.EstimatedProfits + "\n";
                    this._testLogger.PrependLog(log);*/
                }
            }
            catch (Exception ex)
            {
                Debug.Write("Handle Event: ExchangeMarketUpdatedDomainEvent." +
                    "Result: Failure." +
                    "Error Message: " + ex.Message
                    );
                throw ex;
            }
        }
    }
}
