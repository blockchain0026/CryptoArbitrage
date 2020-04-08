using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public class SimpleArbitrage : Entity, IAggregateRoot
    {
        #region Private Fields

        private readonly List<ArbitrageTransaction> _transactions;

        #endregion


        #region Properties
        public string ArbitrageId { get; private set; }
        public ArbitrageBuyOrder BuyOrder { get; private set; }
        public ArbitrageSellOrder SellOrder { get; private set; }
        public decimal EstimateProfits { get; private set; }
        public decimal ActualProfits { get; private set; }
        public ArbitrageData ArbitrageData { get; private set; }

        public SimpleArbitrageStatus Status { get; private set; }
        private int _simpleArbitrageStatusId;

        public bool IsSuccess { get; private set; }
        public string FailureReason { get; private set; }
        public IReadOnlyCollection<ArbitrageTransaction> Transactions => this._transactions;
        #endregion


        #region Constructor
        protected SimpleArbitrage()
        {
            this._transactions = new List<ArbitrageTransaction>();

        }
        private SimpleArbitrage(string arbitrageId, ArbitrageBuyOrder buyOrder, ArbitrageSellOrder sellOrder, decimal estimatedProfits, ArbitrageData arbitrageData, SimpleArbitrageStatus status) : this()
        {
            this.ArbitrageId = arbitrageId ?? throw new ArgumentNullException(nameof(arbitrageId));
            this.BuyOrder = buyOrder ?? throw new ArgumentNullException(nameof(buyOrder));
            this.SellOrder = sellOrder ?? throw new ArgumentNullException(nameof(sellOrder));
            this.EstimateProfits = estimatedProfits;
            this.ArbitrageData = arbitrageData ?? throw new ArgumentNullException(nameof(arbitrageData));
            //this.Status = status ?? throw new ArgumentNullException(nameof(status));
            this._simpleArbitrageStatusId = status != null ? status.Id : throw new ArgumentNullException(nameof(status));
            this.ActualProfits = 0;
            this.IsSuccess = false;
            this.FailureReason = String.Empty;

            this.AddDomainEvent(
                new SimpleArbitrageOpenedDomainEvent(this));
        }
        #endregion


        public static SimpleArbitrage CreateFrom(
            decimal estimatedProfits, string baseCurrency, string quoteCurrency,
            int buyFrom, decimal buyPrice, decimal buyAmounts,
            int sellTo, decimal sellPrice, decimal sellAmounts,
            StopLossSetting buyStopLossSetting, StopLossSetting sellStopLossSetting)
        {
            var arbitrageId = Guid.NewGuid().ToString();

            //無條件捨去至 N 位
            //It seems Binance only support max to 6. 
            int roundedNumber = 6;
            decimal pow = (decimal)Math.Pow(10, roundedNumber);
            decimal roundedBuyAmounts = Math.Floor(buyAmounts * (pow)) / pow;
            decimal roundedSellAmounts = Math.Floor(sellAmounts * (pow)) / pow;

            var buySlipPrice = buyStopLossSetting.GetSlipPrice(buyPrice, baseCurrency, quoteCurrency);
            var buyOrder = new ArbitrageBuyOrder(
                Guid.NewGuid().ToString(),
                buyFrom,
                baseCurrency,
                quoteCurrency,
                buyPrice,
                roundedBuyAmounts,
                buySlipPrice
                );

            var sellSlipPrice = sellStopLossSetting.GetSlipPrice(sellPrice, baseCurrency, quoteCurrency);
            var sellOrder = new ArbitrageSellOrder(
                Guid.NewGuid().ToString(),
                sellTo,
                baseCurrency,
                quoteCurrency,
                sellPrice,
                roundedSellAmounts,
                sellSlipPrice
                );

            var arbitrageData = ArbitrageData.FromSimpleArbitrageOpened(buyOrder, sellOrder);

            return new SimpleArbitrage(
                arbitrageId,
                buyOrder,
                sellOrder,
                estimatedProfits,
                arbitrageData,
                SimpleArbitrageStatus.Opened
                );
        }

        public void OrderCreated(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (order.ArbitrageId != this.ArbitrageId)
                throw new InvalidOperationException($"Created order's arbitrage Id \"{order.ArbitrageId}\" doesn't match the simple arbitrage's Id \"{this.ArbitrageId}\".");
            if (this.Status != SimpleArbitrageStatus.Opened && this.Status != SimpleArbitrageStatus.OrderPartiallyCreated)
                throw new InvalidOperationException("Order can only be created when the simple arbitrage's status is in \"Opened\" or \"OrderPartiallyCreated\".");
            if (order.OrderType == OrderType.BUY_LIMIT && order.OrderId != this.BuyOrder.ArbitrageOrderId)
            {
                throw new InvalidOperationException($"The order's Id \"{order.OrderId}\" doensn't match the arbitrage buy order Id \"{this.BuyOrder.ArbitrageOrderId}\".");
            }
            else if (order.OrderType == OrderType.SELL_LIMIT && order.OrderId != this.SellOrder.ArbitrageOrderId)
            {
                throw new InvalidOperationException($"The order's Id \"{order.OrderId}\" doensn't match the arbitrage buy order Id \"{this.SellOrder.ArbitrageOrderId}\".");
            }


            if (this.Status == SimpleArbitrageStatus.Opened)
            {
                //this.Status = SimpleArbitrageStatus.OrderPartiallyCreated;
                this._simpleArbitrageStatusId = SimpleArbitrageStatus.OrderPartiallyCreated.Id;
            }
            else if (this.Status == SimpleArbitrageStatus.OrderPartiallyCreated)
            {
                //this.Status = SimpleArbitrageStatus.OrderFullCreated;
                this._simpleArbitrageStatusId = SimpleArbitrageStatus.OrderFullCreated.Id;
            }
        }

        public void OrderFilled(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (order.ArbitrageId != this.ArbitrageId)
                throw new InvalidOperationException($"Created order's arbitrage Id \"{order.ArbitrageId}\" doesn't match the simple arbitrage's Id \"{this.ArbitrageId}\".");
            if (order.BaseCurrency != this.ArbitrageData.BaseCurrency || order.QuoteCurrency != this.ArbitrageData.QuoteCurrency)
                throw new InvalidOperationException($"The base currency or quote currency between the filled order and the simple arbitrage must be the same.");
            if (this.Status.Id == SimpleArbitrageStatus.Opened.Id || this.Status.Id == SimpleArbitrageStatus.OrderFullFilled.Id || this.Status.Id == SimpleArbitrageStatus.Closed.Id)
                throw new InvalidOperationException("Order can only be filled when the simple arbitrage's status is in \"OrderPartiallyCreated\" or \"OrderFullCreated\" or \"OrderPartiallyFilled\".");

            var orderFilledAmounts = this.GetOrderFilledAmounts(order.OrderId);
            var trxVolume = order.QuantityFilled - orderFilledAmounts;
            if (trxVolume <= 0)
                throw new InvalidOperationException("The transaction volume must more than zero.");

            var orderPaidFees = this.GetOrderPaidFees(order.OrderId);
            var trxCommisionFees = order.CommisionPaid - orderPaidFees;
            if (trxCommisionFees < 0)
                throw new InvalidOperationException("The transaction fee should be zero or above.");

            var transaction = new ArbitrageTransaction(
                this.ArbitrageId,
                order.ExchangeId,
                order.OrderType.Id,
                order.BaseCurrency,
                order.QuoteCurrency,
                order.Price,
                trxVolume,
                trxCommisionFees
                );

            this._transactions.Add(transaction);
            this.ArbitrageData = this.ArbitrageData.CalculateWithTransaction(transaction);


            if (order.OrderType == OrderType.BUY_LIMIT)
            {
                if (this.GetOrderFilledAmounts(order.OrderId) >= this.BuyOrder.Quantity)
                {
                    if (this.Status.Id == SimpleArbitrageStatus.OrderPartiallyFilled.Id)
                    {
                        //this.Status = SimpleArbitrageStatus.OrderFullFilled;
                        this._simpleArbitrageStatusId = SimpleArbitrageStatus.OrderFullFilled.Id;
                    }
                    else
                    {
                        //this.Status = SimpleArbitrageStatus.OrderPartiallyFilled;
                        this._simpleArbitrageStatusId = SimpleArbitrageStatus.OrderPartiallyFilled.Id;
                    }
                }
            }
            if (order.OrderType == OrderType.SELL_LIMIT)
            {
                if (this.GetOrderFilledAmounts(order.OrderId) >= this.SellOrder.Quantity)
                {
                    if (this.Status.Id == SimpleArbitrageStatus.OrderPartiallyFilled.Id)
                    {
                        //this.Status = SimpleArbitrageStatus.OrderFullFilled;
                        this._simpleArbitrageStatusId = SimpleArbitrageStatus.OrderFullFilled.Id;
                    }
                    else
                    {
                        //this.Status = SimpleArbitrageStatus.OrderPartiallyFilled;
                        this._simpleArbitrageStatusId = SimpleArbitrageStatus.OrderPartiallyFilled.Id;
                    }

                }
            }
        }

        public void OrderCanceled(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (order.ArbitrageId != this.ArbitrageId)
                throw new InvalidOperationException($"Created order's arbitrage Id \"{order.ArbitrageId}\" doesn't match the simple arbitrage's Id \"{this.ArbitrageId}\".");
            if (this.Status.Id != SimpleArbitrageStatus.Opened.Id
                && this.Status.Id != SimpleArbitrageStatus.OrderPartiallyCreated.Id
                && this.Status.Id != SimpleArbitrageStatus.OrderFullCreated.Id
                && this.Status.Id != SimpleArbitrageStatus.OrderPartiallyFilled.Id)
                throw new InvalidOperationException($"Order can not be cancel when in arbitrage status {this.Status.Name}.");
            if (order.OrderType.Id == OrderType.BUY_LIMIT.Id && order.OrderId != this.BuyOrder.ArbitrageOrderId)
            {
                throw new InvalidOperationException($"The order's Id \"{order.OrderId}\" doensn't match the arbitrage buy order Id \"{this.BuyOrder.ArbitrageOrderId}\".");
            }
            else if (order.OrderType.Id == OrderType.SELL_LIMIT.Id && order.OrderId != this.SellOrder.ArbitrageOrderId)
            {
                throw new InvalidOperationException($"The order's Id \"{order.OrderId}\" doensn't match the arbitrage buy order Id \"{this.SellOrder.ArbitrageOrderId}\".");
            }




        }


        public void Close(Order buyOrder, Order sellOrder)
        {
            if (buyOrder == null)
                throw new ArgumentNullException(nameof(buyOrder));
            if (buyOrder.ArbitrageId != this.ArbitrageId)
                throw new InvalidOperationException($"order's arbitrage Id \"{buyOrder.ArbitrageId}\" doesn't match the simple arbitrage's Id \"{this.ArbitrageId}\".");
            if (buyOrder.OrderId != this.BuyOrder.ArbitrageOrderId)
                throw new InvalidOperationException($"Order's Id \"{buyOrder.ArbitrageId}\" doesn't match the arbitrage buy order Id \"{this.BuyOrder.ArbitrageOrderId}\".");
            if (buyOrder.BaseCurrency != this.ArbitrageData.BaseCurrency || buyOrder.QuoteCurrency != this.ArbitrageData.QuoteCurrency)
                throw new InvalidOperationException($"The base currency or quote currency between the buy order and the simple arbitrage must be the same.");

            if (sellOrder == null)
                throw new ArgumentNullException(nameof(sellOrder));
            if (sellOrder.ArbitrageId != this.ArbitrageId)
                throw new InvalidOperationException($"order's arbitrage Id \"{sellOrder.ArbitrageId}\" doesn't match the simple arbitrage's Id \"{this.ArbitrageId}\".");
            if (sellOrder.OrderId != this.SellOrder.ArbitrageOrderId)
                throw new InvalidOperationException($"Order's Id \"{sellOrder.ArbitrageId}\" doesn't match the arbitrage sell order Id \"{this.SellOrder.ArbitrageOrderId}\".");
            if (sellOrder.BaseCurrency != this.ArbitrageData.BaseCurrency || sellOrder.QuoteCurrency != this.ArbitrageData.QuoteCurrency)
                throw new InvalidOperationException($"The base currency or quote currency between the buy order and the simple arbitrage must be the same.");


            bool allowClose = false;

            if (this.Status.Id == SimpleArbitrageStatus.Opened.Id)
            {
                if ((buyOrder.OrderStatus.Id == OrderStatus.Canceled.Id || buyOrder.OrderStatus.Id == OrderStatus.Rejected.Id)
                    && (sellOrder.OrderStatus.Id == OrderStatus.Canceled.Id || sellOrder.OrderStatus.Id == OrderStatus.Rejected.Id))
                {
                        allowClose = true;
                }
                else
                {
                    throw new InvalidOperationException("Simple arbitrage status is in \"opened\", closing is only allowed when both buy and sell orders are canceled or rejected.");
                }
            }
            else if (this.Status.Id == SimpleArbitrageStatus.OrderPartiallyCreated.Id)
            {
                if (buyOrder.OrderStatus.Id == OrderStatus.Canceled.Id && sellOrder.OrderStatus.Id == OrderStatus.Canceled.Id)
                {
                    allowClose = true;
                }
                else
                {
                    throw new InvalidOperationException("Simple arbitrage status is in \"order partially created\", closing is only allowed when both buy and sell orders are canceled.");
                }
            }
            else if (this.Status.Id == SimpleArbitrageStatus.OrderFullCreated.Id)
            {
                if (buyOrder.OrderStatus.Id == OrderStatus.Canceled.Id && sellOrder.OrderStatus.Id == OrderStatus.Canceled.Id)
                {
                    allowClose = true;
                }
                else
                {
                    throw new InvalidOperationException("Simple arbitrage status is in \"order full created\", closing is only allowed when both buy and sell orders are canceled.");
                }
            }
            else if (this.Status.Id == SimpleArbitrageStatus.OrderPartiallyFilled.Id)
            {
                if (buyOrder.OrderStatus.Id == OrderStatus.PartiallyFilled.Id || sellOrder.OrderStatus.Id == OrderStatus.PartiallyFilled.Id)
                {
                    throw new InvalidOperationException("Simple arbitrage status is in \"order partially filled\", closing is not allowed when buy or sell orders are in partially filled status.");
                }
                else
                {
                    allowClose = true;
                }
            }
            else if (this.Status.Id == SimpleArbitrageStatus.OrderFullFilled.Id)
            {
                allowClose = true;
            }

            if (allowClose)
            {
                var baseCurrencyProfits = this.ArbitrageData.GetBaseCurrencyProfits();
                var actualProfits = this.ArbitrageData.GetQuoteCurrencyProfits();

                if (actualProfits <= 0)
                {
                    this.ActualProfits = actualProfits;
                    this.IsSuccess = false;
                    this.FailureReason = "Profits are less than zero.";
                }
                else if (baseCurrencyProfits < 0)
                {
                    this.ActualProfits = actualProfits;
                    this.IsSuccess = false;
                    this.FailureReason = $"The base currency quantity are less than original, lost amount: {Math.Abs(baseCurrencyProfits)}.";
                }
                else
                {
                    this.ActualProfits = actualProfits;
                    this.IsSuccess = true;
                }

                this._simpleArbitrageStatusId = SimpleArbitrageStatus.Closed.Id;
            }
        }




        private decimal GetOrderFilledAmounts(string arbitrageOrderId)
        {
            decimal filledAmounts = 0;
            foreach (var trx in this.Transactions)
            {
                if (trx.ArbitrageOrderId == arbitrageOrderId)
                {
                    filledAmounts += trx.Volume;
                }
            }
            return filledAmounts;
        }

        private decimal GetOrderPaidFees(string arbitrageOrderId)
        {
            decimal fees = 0;
            foreach (var trx in this.Transactions)
            {
                if (trx.ArbitrageOrderId == arbitrageOrderId)
                {
                    fees += trx.CommisionPaid;
                }
            }
            return fees;
        }


    }

}
