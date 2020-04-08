using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Utilitys
{
    public static class ExchangeMarketSymbolParser
    {
        public static string Parse(string exchangeId, string baseCurrency, string quoteCurrency)
        {
            string market = string.Empty;

            if (String.IsNullOrEmpty(baseCurrency) || String.IsNullOrEmpty(quoteCurrency))
            {
                return null;
            }

            if (exchangeId == "4")
            {
                if (quoteCurrency.ToUpper() == "USD")
                {
                    quoteCurrency = "USDT";
                }
                market = (quoteCurrency + "_" + baseCurrency).ToUpper();
            }
            else if (exchangeId == "5")
            {
                if(quoteCurrency.ToUpper()=="USD")
                {
                    quoteCurrency = "USDT";
                }
                market = (baseCurrency + "-" + quoteCurrency).ToUpper();
            }
            return market;
        }

        public static string[] Parse(string exchangeId, string market)
        {
            var baseCurrency = string.Empty;
            var quoteCurrency = string.Empty;

            if (String.IsNullOrEmpty(market))
            {
                return null;
            }

            if (exchangeId == "4")
            {
                quoteCurrency = market.Substring(0, 3);
                baseCurrency = quoteCurrency == "USD" ? market.Substring(5) : market.Substring(4);
            }
            else if(exchangeId=="5")
            {
                quoteCurrency = market.Substring(3);
                baseCurrency = market.Substring(0, 3);
            }

            return new string[] {
                baseCurrency.ToUpper(),
                quoteCurrency.ToUpper()
            };
        }
    }
}
