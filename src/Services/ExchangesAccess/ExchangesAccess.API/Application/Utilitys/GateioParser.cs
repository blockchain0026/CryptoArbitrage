using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using gateio.api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Utilitys
{
    public static class GateioParser
    {

        public static OrderBookViewModel ToOrderBookViewModel(this MarketDeep source)
        {
            var asksList = new List<OrderPriceAndQuantityViewModel>();
            var bidsList = new List<OrderPriceAndQuantityViewModel>();


            foreach (var order in source.Asks)
            {
                asksList.Add(new OrderPriceAndQuantityViewModel
                {
                    price = order.Price,
                    quantity = order.Amount
                });
            }

            foreach (var order in source.Bids)
            {
                bidsList.Add(new OrderPriceAndQuantityViewModel
                {
                    price = order.Price,
                    quantity = order.Amount
                });
            }

            return new OrderBookViewModel
            {
                asks = asksList,
                bids = bidsList
            };
        }

        public static GetOpenOrderViewModel ToGetOpenOrderViewModel(this List<OpenOrderRes> source)
        {
            var orderList = new List<OrderViewModel>();

            foreach (var order in source)
            {
                orderList.Add(new OrderViewModel
                {
                    orderId = order.OrderNumber,
                    quoteCurrency = order.CurrencyPair.Substring(0, 3).ToUpper(),
                    baseCurrency = order.CurrencyPair.Substring(3).ToUpper(),
                    quantityTotal = order.Amount.ToString(),
                    quantityExecuted = order.FilledAmount.ToString(),
                    openedTime = order.Timestamp.ToString(),
                    closedTime = null,
                    canceled = order.Status == "cancelled" ? "true" : "false",
                    commissionPaid = null
                });
            }

            return new GetOpenOrderViewModel
            {
                orders = orderList
            };
        }


        public static BalanceViewModel ToBalanceViewModel(this Balance source)
        {

            List<BalanceDetailViewModel> pairs = new List<BalanceDetailViewModel>();

            foreach (var asset in source.Available)
            {
                decimal locked = 0;
                decimal available = asset.Value;
                source.Locked.TryGetValue(asset.Key, out locked);
                pairs.Add(new BalanceDetailViewModel
                {
                    currency = asset.Key.ToUpper(),
                    balance = (locked + available).ToString(),
                    available = available.ToString(),
                });
            }

            return new BalanceViewModel
            {
                pairs = pairs
            };

        }

    }
}
