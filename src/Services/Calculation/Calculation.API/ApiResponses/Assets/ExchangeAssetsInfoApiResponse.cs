using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.ApiResponses.Assets
{
    public class ExchangeAssetsInfoApiResponse
    {
        public int ExchangeId { get; set; }
        public IEnumerable<AssetApiResponse> Assets { get; set; }
    }
    public class AssetApiResponse
    {
        public string Symbol { get; set; }
        public decimal Balance { get; set; }
    }
}
