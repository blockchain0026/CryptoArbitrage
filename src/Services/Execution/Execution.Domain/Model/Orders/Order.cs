using CryptoArbitrage.Services.Execution.Domain.Events;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Orders
{
    public class Order : Entity, IAggregateRoot
    {
        #region Private Feild

        private DateTime? _dateCreated;
        private DateTime? _dateFilled;
        private DateTime? _dateCanceled;

        #endregion

        #region Properties

        public string OrderId { get; private set; }
        public string ArbitrageId { get; private set; }
        //public Exchange Exchange { get; private set; }
        public int ExchangeId { get; private set; }
        public string ExchangeOrderId { get; private set; }

        public OrderType OrderType { get; private set; }
        private int _orderTypeId;

        public OrderStatus OrderStatus { get; private set; }
        private int _orderStatusId;

        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal QuantityTotal { get; private set; }
        public decimal QuantityFilled { get; private set; }
        public decimal CommisionPaid { get; private set; }
        #endregion

        #region Constructor
        protected Order()
        {

        }
        public Order(string arbitrageOrderId,string arbitrageId, int exchangeId, int orderTypeId, string baseCurrency, string quoteCurrency, decimal price, decimal quantityTotal) : this()
        {
            //this.OrderId = Guid.NewGuid().ToString();

            //The OrderId should be generated from SimpleArbitrage.
            this.OrderId = arbitrageOrderId;

            this.ArbitrageId = arbitrageId ?? throw new ArgumentNullException(nameof(arbitrageId));
            //this.Exchange = exchange !=null?exchange: throw new ArgumentNullException(nameof(exchange));
            this.ExchangeId = exchangeId;
            this.BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            this.QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            this.Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            this.QuantityTotal = quantityTotal > 0 ? quantityTotal : throw new ArgumentOutOfRangeException(nameof(quantityTotal));

            this.QuantityFilled = 0;
            this.CommisionPaid = 0;

            this._orderTypeId = orderTypeId;

            //this.OrderStatus = OrderStatus.Started;
            this._orderStatusId = OrderStatus.Started.Id;

            this.AddDomainEvent(new OrderStartedDomainEvent(
                this.OrderId,
                this.ArbitrageId,
                new Exchange(this.ExchangeId),
                OrderType.From(this._orderTypeId),
                this.BaseCurrency,
                this.QuoteCurrency,
                this.Price,
                this.QuantityTotal
                ));
        }
        #endregion


        public void SubmitOrderToExchange(IExchangeService exchangeService)
        {
            if (this.OrderStatus.Id != OrderStatus.Started.Id)
                throw new InvalidOperationException("The order has already been submitted to exchange, call funtion ResubmitOrderToExchange instead.");
            if (exchangeService == null)
                throw new ArgumentNullException(nameof(exchangeService));

            exchangeService.SubmitOrder(
                new Exchange(this.ExchangeId),
                this.OrderType,
                this.BaseCurrency,
                this.QuoteCurrency,
                this.Price,
                this.QuantityTotal
                );

            //this.OrderStatus = OrderStatus.Submitted;
            this._orderStatusId = OrderStatus.Submitted.Id;

            this.AddDomainEvent(new OrderSubmittedDomainEvent(
                this.OrderId,
                this.ArbitrageId,
                new Exchange(this.ExchangeId),
                this.OrderType,
                this.BaseCurrency,
                this.QuoteCurrency,
                this.Price,
                this.QuantityTotal
                ));
        }
        public void ExchangeOrderSubmitted()
        {
            if (this.OrderStatus.Id != OrderStatus.Started.Id)
                throw new InvalidOperationException("The order has already been submitted to exchange, call funtion ResubmitOrderToExchange instead.");

            //this.OrderStatus = OrderStatus.Submitted;
            this._orderStatusId = OrderStatus.Submitted.Id;

            this.AddDomainEvent(new OrderSubmittedDomainEvent(
                this.OrderId,
                this.ArbitrageId,
                new Exchange(this.ExchangeId),
                this.OrderType,
                this.BaseCurrency,
                this.QuoteCurrency,
                this.Price,
                this.QuantityTotal
                ));
        }

        public void CancelExchangeOrder(IExchangeService exchangeService)
        {
            if (this.OrderStatus == OrderStatus.Started)
                throw new InvalidOperationException("Order could not be cancel if it hasn't been submitted.");
            if (this.OrderStatus == OrderStatus.Canceled)
                throw new InvalidOperationException("The order has already been canceled.");
            if (this.OrderStatus == OrderStatus.Filled)
                throw new InvalidOperationException("Order could not be cancel if it was full filled.");
            if (this.OrderStatus == OrderStatus.Rejected)
                throw new InvalidOperationException("Order could not be cancel if it has been rejected.");

            if (exchangeService == null)
                throw new ArgumentNullException(nameof(exchangeService));

            var success = exchangeService.CancelOrder(new Exchange(this.ExchangeId), this.ExchangeOrderId, this.BaseCurrency, this.QuoteCurrency);
            if (success)
            {
                this._dateCanceled = DateTime.UtcNow;
                //this.OrderStatus = OrderStatus.Canceled;
                this._orderStatusId = OrderStatus.Canceled.Id;

                this.AddDomainEvent(new OrderCanceledDomainEvent(
                    this.OrderId,
                    this.ArbitrageId,
                    new Exchange(this.ExchangeId),
                    this.ExchangeOrderId,
                    this.BaseCurrency,
                    this.QuoteCurrency,
                    this.Price,
                    this.QuantityTotal,
                    this.QuantityFilled,
                    this._dateCanceled ?? throw new FieldAccessException(nameof(this._dateCanceled))
                    ));
            }
            else
            {
                throw new Exception("Order cancelation failed.");
            }
        }

        public void ExchangeOrderCanceled(string exchangeOrderId, DateTime dateCanceled)
        {
            if (this.OrderStatus.Id == OrderStatus.Started.Id)
                throw new InvalidOperationException("Order could not be cancel if it hasn't been submitted.");
            if (this.OrderStatus.Id == OrderStatus.Canceled.Id)
                throw new InvalidOperationException("The order has already been canceled.");
            if (this.OrderStatus.Id == OrderStatus.Filled.Id)
                throw new InvalidOperationException("Order could not be cancel if it was full filled.");
            if (this.OrderStatus.Id == OrderStatus.Rejected.Id)
                throw new InvalidOperationException("Order could not be cancel if it has been rejected.");

            this._dateCanceled = dateCanceled;
            //this.OrderStatus = OrderStatus.Canceled;
            this._orderStatusId = OrderStatus.Canceled.Id;

            this.AddDomainEvent(new OrderCanceledDomainEvent(
                this.OrderId,
                this.ArbitrageId,
                new Exchange(this.ExchangeId),
                this.ExchangeOrderId,
                this.BaseCurrency,
                this.QuoteCurrency,
                this.Price,
                this.QuantityTotal,
                this.QuantityFilled,
                this._dateCanceled ?? throw new FieldAccessException(nameof(this._dateCanceled))
                ));
        }

        public void ExchangeOrderRejected(string exchangeOrderId)
        {
            if (this.OrderStatus.Id != OrderStatus.Submitted.Id)
                throw new InvalidOperationException("Order rejected only happend after submitted and before created.");
            //this.OrderStatus = OrderStatus.Rejected;
            this._orderStatusId = OrderStatus.Rejected.Id;

            this.ExchangeOrderId = exchangeOrderId ?? throw new ArgumentNullException(nameof(exchangeOrderId));
            this.AddDomainEvent(new OrderRejectedDomainEvent(
                this.OrderId,
                this.ArbitrageId,
                new Exchange(this.ExchangeId),
                this.ExchangeOrderId
                ));
        }

        public void ExchangeOrderCreated(string exchangeOrderId, DateTime dateCreated)
        {
            if (this.OrderStatus.Id != OrderStatus.Submitted.Id)
                throw new InvalidOperationException("Order can not be created if it hasn't been submitted to an exchange.");
            this.ExchangeOrderId = exchangeOrderId ?? throw new ArgumentNullException(nameof(exchangeOrderId));
            this._dateCreated = dateCreated != null ? dateCreated : throw new ArgumentNullException(nameof(dateCreated));

            //this.OrderStatus = OrderStatus.Created;
            this._orderStatusId = OrderStatus.Created.Id;

            this.AddDomainEvent(new OrderCreatedDomainEvent(
                this.OrderId,
                this.ArbitrageId,
                new Exchange(this.ExchangeId),
                this.ExchangeOrderId,
                this._dateCreated ?? throw new FieldAccessException(nameof(this._dateCreated))
                ));
        }

        public void ExchangeOrderFilled(decimal qtyExecuted, decimal commisionPaid, DateTime dateFilled)
        {
            if (this.OrderStatus.Id == OrderStatus.Started.Id)
                throw new InvalidOperationException("Order could not be filled if it hasn't been submitted.");
            if (this.OrderStatus.Id == OrderStatus.Submitted.Id)
                throw new InvalidOperationException("Order could not be filled if it hasn't been created.");
            if (this.OrderStatus.Id == OrderStatus.Filled.Id)
                throw new InvalidOperationException("Order could not be filled if it was already full filled.");

            var quantityRemaining = this.QuantityTotal - this.QuantityFilled;
            /*if (qtyExecuted > quantityRemaining)
                throw new ArgumentOutOfRangeException($"The quantity to fill must less than the remaining quantity.");*/

            this.QuantityFilled += qtyExecuted;
            this.CommisionPaid += commisionPaid;
            this._dateFilled = dateFilled;

            this.AddDomainEvent(new OrderPartiallyFilledDomainEvent(
               this.OrderId,
               this.ArbitrageId,
               new Exchange(this.ExchangeId),
               this.OrderType,
               this.BaseCurrency,
               this.QuoteCurrency,
               this.Price,
               this.QuantityTotal,
               this.QuantityFilled,
               this.CommisionPaid,
               this._dateFilled ?? throw new FieldAccessException(nameof(this._dateFilled))
               ));

            //this.OrderStatus = OrderStatus.PartiallyFilled;
            this._orderStatusId = OrderStatus.PartiallyFilled.Id;
        }

        public void TryMarkAsFullFilled()
        {
            if (this.OrderStatus.Id == OrderStatus.PartiallyFilled.Id)
            {
                if (this.QuantityFilled >= this.QuantityTotal)
                {
                    this.AddDomainEvent(new OrderFullFilledDomainEvent(
                        this.OrderId,
                        this.ArbitrageId,
                        new Exchange(this.ExchangeId),
                        this.ExchangeOrderId,
                        this._dateFilled ?? throw new FieldAccessException(nameof(this._dateFilled))
                        ));

                    //this.OrderStatus = OrderStatus.Filled;
                    this._orderStatusId = OrderStatus.Filled.Id;
                }
            }
        }
    }
}
