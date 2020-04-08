using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Commands
{
    public class PlaceBuyOrderCommand : IRequest<bool>
    {

        [DataMember]
        public string ExchangeId { get; private set; }
        [DataMember]
        public string BaseCurrency { get; private set; }

        [DataMember]
        public string QuoteCurrency { get; private set; }

        [DataMember]
        public string Quantity { get; private set; }

        [DataMember]
        public string Price { get; private set; }



        public PlaceBuyOrderCommand(string exchangeId, string baseCurrency, string quoteCurrency, string quantity, string price)
        {
            ExchangeId = exchangeId;
            BaseCurrency = baseCurrency;
            QuoteCurrency = quoteCurrency;
            Quantity = quantity;
            Price = price;
        }

    }
}
