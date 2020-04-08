using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.Models
{
    public class OrderBookViewModel
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
}
