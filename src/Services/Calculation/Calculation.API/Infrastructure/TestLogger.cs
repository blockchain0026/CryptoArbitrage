using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Infrastructure
{
    public class TestLogger : ITestLogger
    {
        private int count = 1;
        private string _testLogger { get; set; }
        private string _prepend { get; set; }
        public TestLogger()
        {
            _testLogger = "Arbitrage founded profits logs:";
            _prepend = string.Empty;
        }

        public void AddLog(string log)
        {
            _testLogger += "\n \n" + count + ". \n" + log;
            count++;
            Console.WriteLine(_testLogger);
        }

        public void PrependLog(string log)
        {
            _prepend = log;
        }

        public string GetLog()
        {
            return this._prepend + "\n"
                + this._testLogger;
        }

    }
}
