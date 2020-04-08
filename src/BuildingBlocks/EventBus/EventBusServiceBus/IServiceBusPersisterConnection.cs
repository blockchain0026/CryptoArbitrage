using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoArbitrage.EventBusServiceBus
{
    public interface IServiceBusPersisterConnection : IDisposable
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ITopicClient CreateModel();
    }
}
