using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {

        [DataMember]
        public string ExchangeId { get; private set; }
        [DataMember]
        public string OrderId { get; private set; }
        [DataMember]
        public string BaseCurrency { get; private set; }
        [DataMember]
        public string QuoteCurrency { get; private set; }
    }
}
