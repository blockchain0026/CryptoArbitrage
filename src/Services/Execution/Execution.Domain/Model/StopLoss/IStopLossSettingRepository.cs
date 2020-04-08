using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.Domain.Model.StopLoss
{
    public interface IStopLossSettingRepository : IRepository<StopLossSetting>
    {
        StopLossSetting Add(StopLossSetting stopLossSetting);
        void Update(StopLossSetting stopLossSetting);
        Task<StopLossSetting> GetByExchangeAsync(Exchange exchange);
        Task<StopLossSetting> GetByIdAsync(string id);
    }
}
