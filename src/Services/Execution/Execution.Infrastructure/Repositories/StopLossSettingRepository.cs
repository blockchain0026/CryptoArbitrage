using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.Model.StopLoss;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Infrastructure.Repositories
{
    public class StopLossSettingRepository : IStopLossSettingRepository
    {
        private readonly ExecutionContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public StopLossSettingRepository(ExecutionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public StopLossSetting Add(StopLossSetting stopLossSetting)
        {
            return _context.StopLossSettings.Add(stopLossSetting).Entity;

        }

        public void Update(StopLossSetting stopLossSetting)
        {
            _context.Entry(stopLossSetting).State = EntityState.Modified;
        }

        public async Task<StopLossSetting> GetByExchangeAsync(Exchange exchange)
        {
            var setting = await _context.StopLossSettings
                .Include(s => s.SlipPrices)
                .Where(s => s.Exchange.ExchangeId == exchange.ExchangeId)
                .SingleOrDefaultAsync();


            return setting;
        }

        public async Task<StopLossSetting> GetByIdAsync(string id)
        {
            var setting = await _context.StopLossSettings
                .Include(s => s.SlipPrices)
                .Where(s => s.Id == int.Parse(id))
                .SingleOrDefaultAsync();

            return setting;
        }
    }
}
