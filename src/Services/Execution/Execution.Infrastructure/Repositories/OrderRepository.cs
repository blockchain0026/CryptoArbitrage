using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Infrastructure.Repositories
{
    public class OrderRepository
     : IOrderRepository
    {
        private readonly ExecutionContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public OrderRepository(ExecutionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Order Add(Order order)
        {
            if (order.IsTransient())
            {
                return _context.Orders
                    .Add(order)
                    .Entity;
            }
            else
            {
                return order;
            }
            //return _context.Orders.Add(order).Entity;

        }

        public async Task<Order> GetAsync(string orderId)
        {
            var orders = _context.Orders;
            var order = await _context.Orders.Where(o => o.OrderId == orderId).SingleOrDefaultAsync();
            var testOrder=orders.FirstOrDefault();
            if (order != null)
            {
                await _context.Entry(order)
                    .Reference(i => i.OrderStatus).LoadAsync();
                await _context.Entry(order)
                    .Reference(i => i.OrderType).LoadAsync();
                /*await _context.Entry(order)
                  .Reference(i => i.Exchange).LoadAsync();*/
            }

            return order;
        }

        public async Task<Order> GetByExchangeOrderIdAsync(string exchangeOrderId)
        {
            var order = await _context.Orders
                .Where(o => o.ExchangeOrderId == exchangeOrderId)
                .SingleOrDefaultAsync();

            return order;
        }

        public Order Update(Order order)
        {
            //_context.Entry(order).State = EntityState.Modified;
            return _context.Orders.Update(order).Entity;
        }
    }
}
