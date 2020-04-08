using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.Domain.Model.Profits
{
    public interface IProfitCalculator
    {
        Task FindProfits(string baseCurrency, string quoteCurrency);
    }
}
