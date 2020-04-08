using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public class SimpleArbitrageStatus : Enumeration
    {

        public static SimpleArbitrageStatus Opened = new SimpleArbitrageStatus(1, nameof(Opened).ToLowerInvariant());
        public static SimpleArbitrageStatus OrderPartiallyCreated = new SimpleArbitrageStatus(2, nameof(OrderPartiallyCreated).ToLowerInvariant());
        public static SimpleArbitrageStatus OrderFullCreated = new SimpleArbitrageStatus(3, nameof(OrderFullCreated).ToLowerInvariant());
        public static SimpleArbitrageStatus OrderPartiallyFilled = new SimpleArbitrageStatus(4, nameof(OrderPartiallyFilled).ToLowerInvariant());
        public static SimpleArbitrageStatus OrderFullFilled = new SimpleArbitrageStatus(5, nameof(OrderFullFilled).ToLowerInvariant());
        public static SimpleArbitrageStatus Closed = new SimpleArbitrageStatus(6, nameof(Closed).ToLowerInvariant());

        protected SimpleArbitrageStatus()
        {
        }

        public SimpleArbitrageStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<SimpleArbitrageStatus> List() =>
            new[] { Opened, OrderPartiallyCreated, OrderFullCreated, OrderPartiallyFilled, OrderFullFilled, Closed };

        public static SimpleArbitrageStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static SimpleArbitrageStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}