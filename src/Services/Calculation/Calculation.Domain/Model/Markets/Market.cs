using CryptoArbitrage.Services.Calculation.Domain.Events;
using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Markets
{
    public class Market : Entity, IAggregateRoot
    {
        public MarketId MarketId { get; private set; }
        public int OrderSizeLimit { get; private set; }


        private readonly List<MarketOrder> _buyOrders;
        public IEnumerable<MarketOrder> BuyOrders => _buyOrders.AsReadOnly();

        private readonly List<MarketOrder> _sellOrders;
        public IEnumerable<MarketOrder> SellOrders => _sellOrders.AsReadOnly();


        private readonly object sync = new object();

        protected Market()
        {
            _buyOrders = new List<MarketOrder>();
            _sellOrders = new List<MarketOrder>();
        }

        public Market(string baseCurrency, string quoteCurrency, int orderSizeLimit) : this()
        {
            this.MarketId = new MarketId(baseCurrency, quoteCurrency);
            this.OrderSizeLimit = orderSizeLimit > 0 ? orderSizeLimit : throw new ArgumentOutOfRangeException(nameof(orderSizeLimit));
        }

        public void UpdateExchangeMarket(Exchange exchange, IEnumerable<OrderPriceAndQuantity> buyOrders, IEnumerable<OrderPriceAndQuantity> sellOrders)
        {


            var orderExchangeId = exchange != null ? exchange.ExchangeId : throw new ArgumentOutOfRangeException(nameof(exchange));

            //Remove old exchange market data.
            this._buyOrders.RemoveAll(b => b.ExchangeId == orderExchangeId);
            this._sellOrders.RemoveAll(b => b.ExchangeId == orderExchangeId);
            /*var exchangeBuyOrders = _buyOrders.Where(o => o.ExchangeId != orderExchangeId).ToList();
            var exchangeSellOrders = _sellOrders.Where(o => o.ExchangeId != orderExchangeId).ToList();*/



            //Add new buy orders.
            foreach (var order in buyOrders)
            {
                //bool added = false;
                var newMarketOrder = new MarketOrder(MarketId.BaseCurrency, MarketId.QuoteCurrency, order.Price, order.Quantity, exchange);

                //Compare to existing orders.
                /*foreach (var existingOrder in exchangeBuyOrders)
                {

                    //Alter index
                    if (MarketOrder.IsBuyingCostLessThan(newMarketOrder, existingOrder))
                    {
                        var index = existingOrder.Index;
                        exchangeBuyOrders.Add(newMarketOrder.ChangeIndex(index));
                        existingOrder = existingOrder.ChangeIndex(index + 1);
                        added = true;
                    }

                }*/

                //exchangeBuyOrders.Add(newMarketOrder);
                this._buyOrders.Add(newMarketOrder);
            }



            //Add new sell orders.
            foreach (var order in sellOrders)
            {
                var newMarketOrder = new MarketOrder(MarketId.BaseCurrency, MarketId.QuoteCurrency, order.Price, order.Quantity, exchange);
                //exchangeSellOrders.Add(newMarketOrder);
                this._sellOrders.Add(newMarketOrder);
            }



            /*_buyOrders = exchangeBuyOrders;
            _sellOrders = exchangeSellOrders;*/


            this.SortOrdersIndex();
            this.LimitOrdersSize();

            //Pulish event.
            this.AddDomainEvent(new ExchangeMarketUpdatedDomainEvent(this, exchange.ExchangeId));
        }




        private void SortOrdersIndex()
        {
            try
            {
                if (_buyOrders.Count > 0)
                {
                    this._buyOrders.Sort(new MarketSellOrderComparaer());


                    this._buyOrders.Reverse();
                }
                if (_sellOrders.Count > 0)
                {
                    this._sellOrders.Sort(new MarketBuyOrderComparaer());


                    this._sellOrders.Reverse();
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void LimitOrdersSize()
        {
            int count;
            count = this._buyOrders.Count;
            if (count > this.OrderSizeLimit)
                _buyOrders.RemoveRange(OrderSizeLimit, count - OrderSizeLimit);
            count = this._sellOrders.Count;
            if (count > this.OrderSizeLimit)
                _sellOrders.RemoveRange(OrderSizeLimit, count - OrderSizeLimit);
        }


        class MarketBuyOrderComparaer : IComparer<MarketOrder>
        {
            public int Compare(MarketOrder x, MarketOrder y)
            {
                return x.IsBuyingCostLessThan(y);
            }
        }

        class MarketSellOrderComparaer : IComparer<MarketOrder>
        {
            public int Compare(MarketOrder x, MarketOrder y)
            {

                return x.IsSellingProfitsMoreThan(y);
            }
        }
    }
}
