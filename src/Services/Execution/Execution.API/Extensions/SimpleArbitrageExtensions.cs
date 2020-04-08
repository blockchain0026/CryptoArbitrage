using CryptoArbitrage.Services.Execution.API.Responses.SimpleArbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.API.Extensions
{
    public static class SimpleArbitrageExtensions
    {
        public static SimpleArbitrageResponse ToSimpleArbitrageResponse (this SimpleArbitrage simpleArbitrage)
        {
            return new SimpleArbitrageResponse
            {
                ArbitrageId=simpleArbitrage.ArbitrageId,
                BuyOrder=simpleArbitrage.BuyOrder,
                SellOrder=simpleArbitrage.SellOrder,
                EstimateProfits=simpleArbitrage.EstimateProfits,
                ActualProfits=simpleArbitrage.ActualProfits,
                ArbitrageData=simpleArbitrage.ArbitrageData,
                Status=simpleArbitrage.Status.Name,
                IsSuccess=simpleArbitrage.IsSuccess,
                FailureReason=simpleArbitrage.FailureReason
            };
        }
    }
}
