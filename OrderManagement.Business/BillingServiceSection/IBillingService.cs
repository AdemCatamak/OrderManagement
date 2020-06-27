using System.Threading.Tasks;

namespace OrderManagement.Business.BillingServiceSection
{
    public interface IBillingService
    {
        Task CreateBill(string correlationId,decimal totalAmount, string buyerName);
        Task CancelBillAsync(string correlationId);
    }
}