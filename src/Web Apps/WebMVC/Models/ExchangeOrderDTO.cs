using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.WebMVC.Models
{
    public class ExchangeOrderDTO
    {

        public string OrderId { get; set; }
        public string ArbitrageId { get; set; }
        public int ExchangeId { get; set; }
        public string ExchangeOrderId { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal Price { get; set; }
        public decimal QuantityTotal { get; set; }
        public decimal QuantityFilled { get; set; }
        public decimal CommisionPaid { get; set; }
    }
}
