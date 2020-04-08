using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.WebMVC.Models
{
    public class SimpleArbitrageDTO
    {
        public string ArbitrageId { get; set; }
        public decimal EstimateProfits { get; set; }
        public decimal ActualProfits { get; set; }
       // public ArbitrageData ArbitrageData { get; set; }

        public string Status { get; set; }

        public bool IsSuccess { get; set; }
        public string FailureReason { get; set; }
    }
}
