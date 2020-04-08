using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages
{
    public interface ISimpleArbitrageRepository : IRepository<SimpleArbitrage>
    {
        SimpleArbitrage Add(SimpleArbitrage simpleArbitrage);
        SimpleArbitrage Update(SimpleArbitrage simpleArbitrage);
        Task<SimpleArbitrage> GetAsync(string arbitrageId);
    }
}
