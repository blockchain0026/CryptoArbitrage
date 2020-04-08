using System.Threading.Tasks;

namespace CryptoArbitrage.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
