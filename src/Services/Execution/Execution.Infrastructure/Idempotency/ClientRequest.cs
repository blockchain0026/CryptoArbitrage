using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.Services.Execution.Infrastructure.Idempotency
{
    public class ClientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}
