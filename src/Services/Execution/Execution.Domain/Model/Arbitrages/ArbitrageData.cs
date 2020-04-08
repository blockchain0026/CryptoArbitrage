using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public class ArbitrageData
    {
        public string BaseCurrency { get; private set; }
        public string QuoteCurrency { get; private set; }
        public decimal OriginalBaseCurrencyQuantity { get; private set; }
        public decimal OriginalQuoteCurrencyQuantity { get; private set; }
        public decimal FinalBaseCurrencyQuantity { get; private set; }
        public decimal FinalQuoteCurrencyQuantity { get; private set; }


        private ArbitrageData(string baseCurrency, string quoteCurrency, decimal originalBaseCurrencyQuantity, decimal originalQuoteCurrencyQuantity)
        {
            this.BaseCurrency = baseCurrency;
            this.QuoteCurrency = quoteCurrency;
            this.OriginalBaseCurrencyQuantity = originalBaseCurrencyQuantity;
            this.OriginalQuoteCurrencyQuantity = originalQuoteCurrencyQuantity;
            this.FinalBaseCurrencyQuantity = OriginalBaseCurrencyQuantity;
            this.FinalQuoteCurrencyQuantity = OriginalQuoteCurrencyQuantity;
        }

        private ArbitrageData(
            string baseCurrency, string quoteCurrency, decimal originalBaseCurrencyQuantity, decimal originalQuoteCurrencyQuantity,
            decimal finalBaseCurrencyQty, decimal finalQuoteCurrencyQty)
            : this(baseCurrency, quoteCurrency, originalBaseCurrencyQuantity, originalQuoteCurrencyQuantity)
        {
            this.FinalBaseCurrencyQuantity = finalBaseCurrencyQty;
            this.FinalQuoteCurrencyQuantity = finalQuoteCurrencyQty;
        }


        public static ArbitrageData FromSimpleArbitrageOpened(ArbitrageBuyOrder buyOrder, ArbitrageSellOrder sellOrder)
        {
            if (buyOrder == null)
                throw new ArgumentNullException(nameof(buyOrder));
            if (sellOrder == null)
                throw new ArgumentNullException(nameof(sellOrder));

            var baseCurrency =
                buyOrder.BaseCurrency == sellOrder.BaseCurrency ?
                buyOrder.BaseCurrency : throw new InvalidOperationException("The base currency between buy order and sell order must be the same currency.");
            var quoteCurrency =
                buyOrder.QuoteCurrency == sellOrder.QuoteCurrency ?
                buyOrder.QuoteCurrency : throw new InvalidOperationException("The quote currency between buy order and sell order must be the same currency.");

            //Defaults to zero.
            decimal originalBaseCurrencyQty = 0;
            decimal originalQuoteCurrencyQty = 0;

            originalQuoteCurrencyQty += buyOrder.Price * buyOrder.Quantity;
            originalBaseCurrencyQty += sellOrder.Quantity;

            return new ArbitrageData(baseCurrency, quoteCurrency, originalBaseCurrencyQty, originalQuoteCurrencyQty);
        }

        public ArbitrageData CalculateWithTransaction(ArbitrageTransaction arbitrageTransaction)
        {
            if (arbitrageTransaction == null)
                throw new ArgumentNullException(nameof(arbitrageTransaction));
            if (arbitrageTransaction.OriginalOrderType == null)
                throw new ArgumentNullException(nameof(arbitrageTransaction.OriginalOrderType));

            if (arbitrageTransaction.OriginalOrderType == OrderType.BUY_LIMIT)
            {
                var baseCurrencyFee = arbitrageTransaction.CommisionPaid;
                var baseCurrencyGet = arbitrageTransaction.Volume - baseCurrencyFee;
                var quoteCurrencyLost = arbitrageTransaction.Price * arbitrageTransaction.Volume;

                decimal finalBaseCurrencyQty = this.FinalBaseCurrencyQuantity + baseCurrencyGet;
                decimal finalQuoteCurrencyQty = this.FinalQuoteCurrencyQuantity - quoteCurrencyLost;

                return new ArbitrageData(
                    this.BaseCurrency,
                    this.QuoteCurrency,
                    this.OriginalBaseCurrencyQuantity,
                    this.OriginalQuoteCurrencyQuantity,
                    finalBaseCurrencyQty,
                    finalQuoteCurrencyQty
                    );
            }
            else if (arbitrageTransaction.OriginalOrderType == OrderType.SELL_LIMIT)
            {
                var quoteCurrencyFee = arbitrageTransaction.CommisionPaid;
                var quoteCurrencyGet = arbitrageTransaction.Volume - quoteCurrencyFee;
                var baseCurrencyLost = arbitrageTransaction.Volume;

                decimal finalBaseCurrencyQty = this.FinalBaseCurrencyQuantity - baseCurrencyLost;
                decimal finalQuoteCurrencyQty = this.FinalQuoteCurrencyQuantity + quoteCurrencyGet;

                return new ArbitrageData(
                    this.BaseCurrency,
                    this.QuoteCurrency,
                    this.OriginalBaseCurrencyQuantity,
                    this.OriginalQuoteCurrencyQuantity,
                    finalBaseCurrencyQty,
                    finalQuoteCurrencyQty
                    );
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Arbitrage order type \"{arbitrageTransaction.OriginalOrderType}\" is currently not supported.");
            }


        }

        public decimal GetBaseCurrencyProfits()
        {
            return this.FinalBaseCurrencyQuantity - this.OriginalBaseCurrencyQuantity;
        }

        public decimal GetQuoteCurrencyProfits()
        {
            return this.FinalQuoteCurrencyQuantity - this.OriginalQuoteCurrencyQuantity;
        }
    }
}
