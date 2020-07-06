using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Business.ExternalEvents.ShipmentEvents;

namespace OrderManagement.Business.Clients
{
    public interface IShipmentServiceClient
    {
        Task CreateShipmentAsync(string correlationId, string receiverName, string receiverAddress);
    }

    public class ShipmentServiceClient : IShipmentServiceClient
    {
        private readonly ILogger<ShipmentServiceClient> _logger;
        private readonly IBusControl _busControl;

        public ShipmentServiceClient(ILogger<ShipmentServiceClient> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task CreateShipmentAsync(string correlationId, string receiverName, string receiverAddress)
        {
            _logger.LogInformation($"{correlationId} - Shipment is created. Receiver name is {receiverName} and receiver address is {receiverAddress}");
            await _busControl.Publish(new ShipmentCreatedEvent(correlationId));

            await Task.Delay(TimeSpan.FromSeconds(5));

            if (receiverAddress.Length < 5)
            {
                _logger.LogInformation($"{correlationId} - Shipment is returned. Receiver name is {receiverName} and receiver address is {receiverAddress}");
                await _busControl.Publish(new ShipmentReturnedEvent(correlationId));
            }
            else
            {
                _logger.LogInformation($"{correlationId} - Shipment is delivered. Receiver name is {receiverName} and receiver address is {receiverAddress}");
                await _busControl.Publish(new ShipmentDeliveredEvent(correlationId, DateTime.UtcNow));
            }
        }
    }
}