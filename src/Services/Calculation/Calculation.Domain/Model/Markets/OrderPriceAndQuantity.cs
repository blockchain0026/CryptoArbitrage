using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Markets
{
    public class OrderPriceAndQuantity : ValueObject
    {
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
        public OrderPriceAndQuantity(decimal price, decimal quantity)
        {
            Price = price > 0 ? price : throw new ArgumentOutOfRangeException(nameof(price));
            Quantity = quantity > 0 ? quantity : throw new ArgumentOutOfRangeException(nameof(quantity));
            /*if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));
            Quantity = quantity;*/
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Price;
            yield return Quantity;
        }
    }
}
