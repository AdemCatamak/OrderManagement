using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Business.Domain.Exceptions;
using OrderManagement.Data;
using OrderManagement.Data.Models;
using OrderManagement.Utility.IntegrationMessagePublisherSection;

namespace OrderManagement.Business.Domain.OrderStateMachineSection
{
    public class OrderStateMachineFactory : IOrderStateMachineFactory
    {
        private readonly DataContext _dataContext;
        private readonly IIntegrationMessagePublisher _integrationMessagePublisher;

        public OrderStateMachineFactory(DataContext dataContext, IIntegrationMessagePublisher integrationMessagePublisher)
        {
            _dataContext = dataContext;
            _integrationMessagePublisher = integrationMessagePublisher;
        }

        public async Task<IOrderStateMachine> CreateOrderStateMachineAsync(string buyerName, string buyerAddress, decimal totalAmount)
        {
            var orderModel = new OrderModel(buyerName, buyerAddress, totalAmount);
            _dataContext.Add(orderModel);
            await _dataContext.SaveChangesAsync();

            return BuildOrderStateMachine(orderModel);
        }

        public async Task<IOrderStateMachine> BuildOrderStateMachineAsync(long orderId)
        {
            OrderModel orderModel = await _dataContext.OrderModels.FirstOrDefaultAsync(m => m.Id == orderId);
            if (orderModel == null) throw new OrderNotFoundException(orderId);

            return BuildOrderStateMachine(orderModel);
        }

        private IOrderStateMachine BuildOrderStateMachine(OrderModel orderModel)
        {
            var orderStateMachine = new OrderStateMachine(orderModel, _integrationMessagePublisher, _dataContext);
            return orderStateMachine;
        }
    }
}