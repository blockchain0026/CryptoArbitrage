using CryptoArbitrage.Services.Calculation.API.Models;
using CryptoArbitrage.Services.Calculation.Domain.Model.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Extensions
{
    public static class OrderPriceAndQuantityModelExtensions
    {
        public static IEnumerable<OrderPriceAndQuantity> ToOrderPriceAndQuantitys(this IEnumerable<OrderPriceAndQuantityViewModel> orderPriceAndQuantityViewModels)
        {
            foreach (var order in orderPriceAndQuantityViewModels)
            {
                OrderPriceAndQuantity orderToReturn = null;
                try
                {
                    orderToReturn = order.ToOrderPriceAndQuantity();
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }
                yield return orderToReturn;
            }
        }

        public static OrderPriceAndQuantity ToOrderPriceAndQuantity(this OrderPriceAndQuantityViewModel orderPriceAndQuantityViewModel)
        {
            return new OrderPriceAndQuantity(orderPriceAndQuantityViewModel.price, orderPriceAndQuantityViewModel.quantity);
        }
    }
}
