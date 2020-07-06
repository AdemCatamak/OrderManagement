using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderManagement.Utility.IntegrationMessagePublisherSection
{
    public interface IIntegrationMessagePublisher
    {
        IReadOnlyList<IIntegrationMessage> IntegrationMessages { get; }
        void AddMessage(IIntegrationMessage integrationMessage);
        Task Publish(CancellationToken cancellationToken = default);
    }
}