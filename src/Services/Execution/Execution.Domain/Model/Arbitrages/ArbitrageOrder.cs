﻿using System;
using System.Collections.Generic;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public class ArbitrageOrder : ValueObject
    {
        public ArbitrageOrder(string arbitrageOrderId, int exchangeId, int orderTypeId, string baseCurrency, string quoteCurrency, decimal price, decimal quantity, decimal slipPrice)
        {
            ArbitrageOrderId = arbitrageOrderId ?? throw new ArgumentNullException(nameof(arbitrageOrderId));
            ExchangeId = exchangeId>=0 ?exchangeId: throw new ArgumentOutOfRangeException(nameof(exchangeId));
            _orderTypeId = orderTypeId;
            BaseCurrency = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            QuoteCurrency = quoteCurrency ?? throw new ArgumentNullException(nameof(quoteCurrency));
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Quantity = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity));
            SlipPrice = slipPrice;
        }

        public string ArbitrageOrderId { get; private set; }
        public int ExchangeId { get; private set; }
        public OrderType OrderType { get; private set; }
        private int _orderTypeId;
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal SlipPrice { get; private set; }




        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.ArbitrageOrderId;
            yield return this.ExchangeId;
            yield return this.OrderType;
            yield return this.BaseCurrency;
            yield return this.QuoteCurrency;
            yield return this.Price;
            yield return this.Quantity;
            yield return this.SlipPrice;
        }
    }
}