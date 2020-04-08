using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries
{
    public class ExchangeViewModel
    {
        public string exchangeid { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public List<APICommandViewModel> apicommands { get; set; }
        public List<APIResponseViewModel> apiresponses { get; set; }
    }
    public class APICommandViewModel
    {
        public int apicommandtypeid { get; set; }
        public List<CommandParameterViewModel> commandparameters { get; set; }
    }
    public class APIResponseViewModel
    {
        public int apiresponsetypeid { get; set; }
        public List<ResponseParameterViewModel> responseparameters { get; set; }
    }
    public class CommandParameterViewModel
    {
        public string name { get; set; }
        public bool isrequired { get; set; }
        public string description { get; set; }

    }
    public class ResponseParameterViewModel
    {
        public string name { get; set; }
        public string correspondto { get; set; }
    }

    public class OrderBookViewModel : ApiResponse
    {
        /*public string baseCurrency { get; set; }
        public string quoteCurrency { get; set; }*/

        public List<OrderPriceAndQuantityViewModel> asks { get; set; }
        public List<OrderPriceAndQuantityViewModel> bids { get; set; }


    }
    public class OrderPriceAndQuantityViewModel
    {
        public decimal price { get; set; }
        public decimal quantity { get; set; }
    }




    public class BalanceViewModel : ApiResponse
    {

        public List<BalanceDetailViewModel> pairs { get; set; }
    }
    public class BalanceDetailViewModel
    {
        public string currency { get; set; }
        public string balance { get; set; }
        public string available { get; set; }
        public string pending { get; set; }
    }


    public class GetOpenOrderViewModel : ApiResponse
    {
        /*
        "Uuid": null,
        "OrderUuid": "675edeae-665a-43ed-ac8f-f93635a508c7",
        "Exchange": "ETH-ADA",
        "OrderType": "LIMIT_BUY",
        "Quantity": 35.0,
        "QuantityRemaining": 35.0,
        "Limit": 0.0003,
        "QuantityRemaining": 0.0,
        "Price": 0.0,
        "PricePerUnit": null,
        "Opened": "2018-09-03T05:10:49.023",
        "Closed": null,
        "CancelInitiated": false,
        "ImmediateOrCancel": false,
        "IsConditional": false,
        "Condition": "NONE",
        "ConditionTarget": null
         */

        public List<OrderViewModel> orders { get; set; }

    }
    public class OrderViewModel
    {
        public string orderId { get; set; }
        public string baseCurrency { get; set; }
        public string quoteCurrency { get; set; }
        public string quantityTotal { get; set; }
        public string quantityExecuted { get; set; }
        public string commissionPaid { get; set; }
        public string openedTime { get; set; }
        public string closedTime { get; set; }
        public string canceled { get; set; }
    }


    public class PlaceBuyOrderViewModel : ApiResponse
    {
        public string orderId { get; set; }
    }

    public class PlaceSellOrderViewModel : ApiResponse
    {
        public string orderId { get; set; }
    }

    public class CancelOrderViewModel : ApiResponse
    {
        public bool orderCancelled { get; set; }
    }
}
