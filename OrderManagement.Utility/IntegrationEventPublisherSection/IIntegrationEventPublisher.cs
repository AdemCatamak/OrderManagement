using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderManagement.Utility.IntegrationEventPublisherSection
{
    public interface IIntegrationEventPublisher
    {
        IReadOnlyList<IIntegrationEvent> IntegrationEvents { get; }
        void AddEvent(IIntegrationEvent integrationEvent);
        Task Publish(CancellationToken cancellationToken = default);
    }
}