using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Infrastructure
{
    public class ExchangeApiRequestFrequenciesControlService : IExchangeApiRequestFrequenciesControlService
    {
        private Dictionary<int, long> _lastRequests;
        public int IntervalInSeconds { get; private set; }
        private readonly object _sync = new object();

        public ExchangeApiRequestFrequenciesControlService()
        {
            this._lastRequests = new Dictionary<int, long>();
            this.Initialize();
        }

        public void SetMinRequestIntervalInSeconds(int seconds)
        {
            lock (_sync)
            {
                if (seconds < 0)
                {
                    throw new InvalidOperationException("Exchange API request interval must more than 0 in seconds.");
                }
                this.IntervalInSeconds = seconds;
            }
        }

        public void UpdateRequestsTime(int exchangeId)
        {
            lock(_sync)
            {
                long lastRequestUnixTimeSeconds = 0;
                if (this._lastRequests.TryGetValue(exchangeId, out lastRequestUnixTimeSeconds))
                {
                    _lastRequests[exchangeId] = DateTimeOffset.Now.ToUnixTimeSeconds();
                }
            }
       
        }

        public bool IsRequestAllow(int exchangeId)
        {
            lock(_sync)
            {
                long lastRequestUnixTimeSeconds = 0;
                var isFounded = this._lastRequests.TryGetValue(exchangeId, out lastRequestUnixTimeSeconds);
                if (isFounded)
                {
                    var gap = DateTimeOffset.Now.ToUnixTimeSeconds() - lastRequestUnixTimeSeconds;
                    if (gap >= IntervalInSeconds)
                        return true;
                    else
                        return false;
                }
                return false;
            }
        }

        public void Initialize()
        {
            lock(_sync)
            {
                this.IntervalInSeconds = 5;
                for (int i = 0; i < 5; i++)
                {
                    _lastRequests.Add(i, DateTimeOffset.Now.ToUnixTimeSeconds());
                }
            }
        }

    }
}
