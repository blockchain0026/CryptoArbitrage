using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.WebAPI
{
    public class ExecutionSettings
    {
        public bool UseCustomizationData { get; set; }
        public string ConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int GracePeriodTime { get; set; }

        public int CheckUpdateTime { get; set; }
    }
}
