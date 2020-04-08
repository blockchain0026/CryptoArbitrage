using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public class ExchangeApiSetting
    {
        public ExchangeAPI Bittrex { get; set; }
        public ExchangeAPI Binance { get; set; }
        public ExchangeAPI Kraken { get; set; }
        public ExchangeAPI Bitstamp { get; set; }
        public ExchangeAPI DragonEx { get; set; }
        public ExchangeAPI Poloniex { get; set; }
        public ExchangeAPI Gateio { get; set; }
        public ExchangeAPI Kucoin { get; set; }
    }
}
