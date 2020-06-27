using System.Threading.Tasks;

namespace OrderManagement.Business.PaymentServiceSection
{
    public interface IPaymentService
    {
        Task TakePaymentAsync(string correlationId, decimal totalAmount);
        Task RefundAsync(string correlationId);
    }
}