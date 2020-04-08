using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Responses.SimpleArbitrages
{
    public class SimpleArbitrageResponse
    {
        public string ArbitrageId { get; set; }
        public ArbitrageBuyOrder BuyOrder { get; set; }
        public ArbitrageSellOrder SellOrder { get; set; }
        public decimal EstimateProfits { get; set; }
        public decimal ActualProfits { get;set; }
        public ArbitrageData ArbitrageData { get;set; }

        public string Status { get; set; }

        public bool IsSuccess { get; set; }
        public string FailureReason { get; set; }
    }
}
