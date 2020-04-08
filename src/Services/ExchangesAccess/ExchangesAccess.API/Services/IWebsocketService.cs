using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Services
{
    public interface IWebsocketService
    {
        Task<bool> Authenticate(string apiKey, string signedChallenge);
        void Close();
        void Dispose();
        Task<string> GetAuthContext(string apiKey);
        bool IsConnectted(string exchangeId);
        Task<bool> ListenToMarket(string exchangeId, string baseCurrency, string quoteCurrency);
        void Shutdown();
        Task<bool> StartListening();
        Task<bool> SubscribeToExchangeDeltas(string marketName);
    }
}
