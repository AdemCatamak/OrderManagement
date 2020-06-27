using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Business.ExternalEvents.ShipmentEvents;

namespace OrderManagement.Business.ShipmentServiceSection
{
    public class ShipmentService : IShipmentService
    {
        private readonly ILogger<ShipmentService> _logger;
        private readonly IBusControl _busControl;

        public ShipmentService(ILogger<ShipmentService> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task CreateShipmentAsync(string correlationId, string receiverName, string receiverAddress)
        {
            _logger.LogInformation($"{correlationId} - Shipment is created. Receiver name is {receiverName} and address is {receiverAddress}");

            if (receiverAddress.Length < 5)
            {
                await _busControl.Publish(new ShipmentReturnedEvent(correlationId));
                _logger.LogInformation($"{correlationId} - Shipment is returned. Receiver name is {receiverName} and address is {receiverAddress}");
            }
            else
            {
                await _busControl.Publish(new ShipmentDeliveredEvent(correlationId, DateTime.UtcNow));
                _logger.LogInformation($"{correlationId} - Shipment is delivered. Receiver name is {receiverName} and address is {receiverAddress}");
            }
        }
    }
}