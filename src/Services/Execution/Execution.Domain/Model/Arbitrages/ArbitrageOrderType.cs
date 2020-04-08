using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public class ArbitrageOrderType : Enumeration
    {
        public static ArbitrageOrderType BUY_LIMIT = new ArbitrageOrderType(1, nameof(BUY_LIMIT).ToLowerInvariant());
        public static ArbitrageOrderType SELL_LIMIT = new ArbitrageOrderType(2, nameof(SELL_LIMIT).ToLowerInvariant());

        protected ArbitrageOrderType()
        {
        }

        public ArbitrageOrderType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<ArbitrageOrderType> List() =>
            new[] { BUY_LIMIT, SELL_LIMIT };

        public static ArbitrageOrderType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for ArbitrageOrderType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static ArbitrageOrderType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for OrderType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}

