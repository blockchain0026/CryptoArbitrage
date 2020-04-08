using CryptoArbitrage.Services.Calculation.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges
{
    public interface IExchangeRepository : IRepository<Exchange>
    {
        Exchange Add(Exchange exchange);

        void Update(Exchange exchange);

        Task<Exchange> GetAsync(int exchangeId);
    }
}
