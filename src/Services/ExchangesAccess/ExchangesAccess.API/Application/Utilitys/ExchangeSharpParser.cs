using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Utilitys
{
    public static class ExchangeSharpParser
    {

        public static OrderBookViewModel ToOrderBookViewModel(this ExchangeOrderBook source)
        {
            var asksList = new List<OrderPriceAndQuantityViewModel>();
            var bidsList = new List<OrderPriceAndQuantityViewModel>();


            foreach (var order in source.Asks)
            {
                asksList.Add(new OrderPriceAndQuantityViewModel
                {
                    price = Decimal.Parse(order.Value.Price.ToString()),
                    quantity = Decimal.Parse(order.Value.Amount.ToString())
                });
            }

            foreach (var order in source.Bids)
            {
                bidsList.Add(new OrderPriceAndQuantityViewModel
                {
                    price = Decimal.Parse(order.Value.Price.ToString()),
                    quantity = Decimal.Parse(order.Value.Amount.ToString())
                });
            }

            return new OrderBookViewModel
            {
                asks = asksList,
                bids = bidsList
            };
        }

        public static GetOpenOrderViewModel ToGetOpenOrderViewModel(this List<ExchangeOrderResult> source)
        {
            var orderList = new List<OrderViewModel>();

            foreach (var order in source)
            {
                orderList.Add(new OrderViewModel
                {
                    orderId = order.OrderId,
                    quoteCurrency = order.Symbol.Substring(0, 3).ToUpper(),
                    baseCurrency = order.Symbol.Substring(3).ToUpper(),
                    quantityTotal = order.Amount.ToString(),
                    quantityExecuted = order.AmountFilled.ToString(),
                    openedTime = order.OrderDate.Ticks.ToString(),
                    closedTime = order.Result == ExchangeAPIOrderResult.Filled ? order.FillDate.ToString() : null,
                    canceled = order.Result == ExchangeAPIOrderResult.Canceled ? "true" : "false",
                    commissionPaid = order.Fees.ToString()
                });
            }

            return new GetOpenOrderViewModel
            {
                orders = orderList
            };
        }
        public static GetOpenOrderViewModel ToGetOpenOrderViewModel(this List<ExchangeOrderResult> source, string exchangeId)
        {
            var orderList = new List<OrderViewModel>();

            foreach (var order in source)
            {
                var marketSymbol = ExchangeMarketSymbolParser.Parse(exchangeId, order.Symbol);


                orderList.Add(new OrderViewModel
                {
                    orderId = order.OrderId,
                    baseCurrency = marketSymbol[0],
                    quoteCurrency = marketSymbol[1],
                    quantityTotal = order.Amount.ToString(),
                    quantityExecuted = order.AmountFilled.ToString(),
                    openedTime = order.OrderDate.Ticks.ToString(),
                    closedTime = order.Result == ExchangeAPIOrderResult.Filled ? order.FillDate.ToString() : null,
                    canceled = order.Result == ExchangeAPIOrderResult.Canceled ? "true" : "false",
                    commissionPaid = order.Fees.ToString()
                });
            }

            return new GetOpenOrderViewModel
            {
                orders = orderList
            };
        }

        public static PlaceBuyOrderViewModel ToPlaceBuyOrderViewModel(this ExchangeOrderResult source)
        {
            return new PlaceBuyOrderViewModel
            {
                orderId = source.OrderId
            };
        }


        public static PlaceSellOrderViewModel ToPlaceSellOrderViewModel(this ExchangeOrderResult source)
        {
            return new PlaceSellOrderViewModel
            {
                orderId = source.OrderId
            };
        }

        public static bool TryGetBalanceViewModel(out BalanceViewModel balanceViewModel, Dictionary<string, decimal> totalBalances, Dictionary<string, decimal> availableBalances)
        {

            List<BalanceDetailViewModel> pairs = new List<BalanceDetailViewModel>();

            foreach (var asset in totalBalances)
            {
                pairs.Add(new BalanceDetailViewModel
                {
                    currency = asset.Key.ToUpper(),
                    balance = asset.Value.ToString(),
                    available = availableBalances[asset.Key].ToString(),
                });
            }

            if (pairs.Count > 0)
            {

                balanceViewModel = new BalanceViewModel
                {
                    pairs = pairs
                };
                return true;
            }
            else
            {
                balanceViewModel = null;
                return false;
            }
        }

    }
}
