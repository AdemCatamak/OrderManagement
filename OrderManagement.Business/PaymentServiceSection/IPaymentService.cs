using System.Threading.Tasks;

namespace OrderManagement.Business.PaymentServiceSection
{
    public interface IPaymentService
    {
        Task TakePayment(string buyerName, decimal totalAmount);
    }
}