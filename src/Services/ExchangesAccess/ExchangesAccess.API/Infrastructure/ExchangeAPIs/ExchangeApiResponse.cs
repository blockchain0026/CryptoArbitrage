using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public abstract class ApiResponse
    {

    }
    public class ExchangeApiResponse<TResponce> where TResponce : ApiResponse
    {

        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public TResponce Result { get; set; }
        //public long? Size { get; set; }
    }
}
