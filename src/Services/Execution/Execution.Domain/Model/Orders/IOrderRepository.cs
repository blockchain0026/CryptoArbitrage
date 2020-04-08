using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Orders
{
    public interface IOrderRepository:IRepository<Order>
    {
        Order Add(Order order);
        Order Update(Order order);
        Task<Order> GetAsync(string orderId);
        Task<Order> GetByExchangeOrderIdAsync(string exchangeOrderId);
        
    }
}
