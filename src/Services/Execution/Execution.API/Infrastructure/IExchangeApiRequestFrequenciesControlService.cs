namespace CryptoArbitrage.Services.Execution.API.Infrastructure
{
    public interface IExchangeApiRequestFrequenciesControlService
    {
        bool IsRequestAllow(int exchangeId);
        void SetMinRequestIntervalInSeconds(int seconds);
        void UpdateRequestsTime(int exchangeId);
    }
}