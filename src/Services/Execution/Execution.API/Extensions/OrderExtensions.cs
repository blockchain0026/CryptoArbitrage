using CryptoArbitrage.Services.Execution.API.Responses.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Extensions
{
    public static class OrderExtensions
    {
        public static OrderResponse ToOrderResponse(this Order order)
        {
            return new OrderResponse
            {
                OrderId=order.OrderId,
                ArbitrageId=order.ArbitrageId,
                ExchangeId=order.ExchangeId,
                ExchangeOrderId=order.ExchangeOrderId,
                OrderType=order.OrderType.Name,
                OrderStatus=order.OrderStatus.Name,
                BaseCurrency=order.BaseCurrency,
                QuoteCurrency=order.QuoteCurrency,
                Price=order.Price,
                QuantityTotal=order.QuantityTotal,
                QuantityFilled=order.QuantityFilled,
                CommisionPaid=order.CommisionPaid
            };
        }
    }
}
