using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Infrastructure.Repositories
{
    public class SimpleArbitrageRepository
        : ISimpleArbitrageRepository
    {
        private readonly ExecutionContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public SimpleArbitrageRepository(ExecutionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public SimpleArbitrage Add(SimpleArbitrage simpleArbitrage)
        {
            if (simpleArbitrage.IsTransient())
            {
                return _context.SimpleArbitrages
                    .Add(simpleArbitrage)
                    .Entity;
            }
            else
            {
                return simpleArbitrage;
            }
        }

        public SimpleArbitrage Update(SimpleArbitrage simpleArbitrage)
        {
            return _context.SimpleArbitrages
                .Update(simpleArbitrage)
                .Entity;
        }

        public async Task<SimpleArbitrage> GetAsync(string arbitrageId)
        {
            var arbitrage = await _context.SimpleArbitrages
                .Include(s => s.Transactions)
                .Where(s => s.ArbitrageId == arbitrageId)
                .SingleOrDefaultAsync();

            if (arbitrage != null)
            {
                await _context.Entry(arbitrage)
                    .Reference(i => i.Status).LoadAsync();
                /*await _context.Entry(order)
                  .Reference(i => i.Exchange).LoadAsync();*/
            }

            return arbitrage;
        }


    }
}
