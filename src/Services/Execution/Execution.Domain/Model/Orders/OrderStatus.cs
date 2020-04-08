using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Orders
{
    public class OrderStatus
        : Enumeration
    {
        public static OrderStatus Started = new OrderStatus(1, nameof(Started).ToLowerInvariant());
        public static OrderStatus Submitted = new OrderStatus(2, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus Rejected = new OrderStatus(3, nameof(Rejected).ToLowerInvariant());
        public static OrderStatus Created = new OrderStatus(4, nameof(Created).ToLowerInvariant());
        public static OrderStatus PartiallyFilled = new OrderStatus(5, nameof(PartiallyFilled).ToLowerInvariant());
        public static OrderStatus Filled = new OrderStatus(6, nameof(Filled).ToLowerInvariant());
        public static OrderStatus Canceled = new OrderStatus(7, nameof(Canceled).ToLowerInvariant());

        protected OrderStatus()
        {
        }

        public OrderStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<OrderStatus> List() =>
            new[] { Started, Submitted, Rejected, Created, PartiallyFilled, Filled, Canceled };

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
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
