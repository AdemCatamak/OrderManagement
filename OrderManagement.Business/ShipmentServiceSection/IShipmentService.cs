using System.Threading.Tasks;

namespace OrderManagement.Business.ShipmentServiceSection
{
    public interface IShipmentService
    {
        Task CreateShipmentAsync(string correlationId, string receiverName, string receiverAddress);
    }
}