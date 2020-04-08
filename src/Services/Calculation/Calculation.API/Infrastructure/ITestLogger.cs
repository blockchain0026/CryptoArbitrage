using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Infrastructure
{
    public interface ITestLogger
    {
        void AddLog(string log);
        void PrependLog(string log);
        string GetLog();
    }
}
