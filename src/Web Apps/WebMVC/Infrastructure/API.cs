using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.WebMVC.Infrastructure
{
    public static class API
    {
        public static class Calculation
        {
            public static string GetAllAssets(string baseUri)
            {
                return $"{baseUri}/Assets";
            }
        }

        public static class Execution
        {
            public static string GetExchangeOrder(string baseUri, string orderId)
            {
                return $"{baseUri}/Orders/{orderId}";
            }

            public static string GetAllOrders(string baseUri)
            {
                return $"{baseUri}/Orders";
            }

            public static string GetSimpleArbitrage(string baseUri, string simpelArbitrageId)
            {
                return $"{baseUri}/SimpleArbitrages/{simpelArbitrageId}";
            }

            public static string GetAllSimpleArbitrages(string baseUri)
            {
                return $"{baseUri}/SimpleArbitrages";
            }
        }

    

    }
}
