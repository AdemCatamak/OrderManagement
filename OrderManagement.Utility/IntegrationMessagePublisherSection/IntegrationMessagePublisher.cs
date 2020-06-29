using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace OrderManagement.Utility.IntegrationMessagePublisherSection
{
    public class IntegrationMessagePublisher : IIntegrationMessagePublisher,
                                             IDisposable
    {
        private readonly IBusControl _busControl;
        private readonly ConcurrentQueue<IIntegrationMessage> _messages;

        public IntegrationMessagePublisher(IBusControl busControl)
        {
            _busControl = busControl ?? throw new ArgumentNullException(nameof(busControl));
            _messages = new ConcurrentQueue<IIntegrationMessage>();
        }

        public IReadOnlyList<IIntegrationMessage> IntegrationMessages => _messages.ToList().AsReadOnly();

        public void AddMessage(IIntegrationMessage integrationMessage)
        {
            _messages.Enqueue(integrationMessage);
        }

        public async Task Publish(CancellationToken cancellationToken = default)
        {
            var publishTasks = new List<Task>();

            while (_messages.TryDequeue(out IIntegrationMessage integrationEvent))
            {
                Task publishTask = _busControl.Publish(integrationEvent, integrationEvent.GetType(), cancellationToken);
                publishTasks.Add(publishTask);
            }

            await Task.WhenAll(publishTasks);
        }

        public void Dispose()
        {
            _messages.Clear();
        }
    }
}