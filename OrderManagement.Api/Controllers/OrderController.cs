using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Contracts.OrderRequests;
using OrderManagement.Business.Domain.OrderServiceSection;
using OrderManagement.Business.Domain.OrderServiceSection.Requests;
using OrderManagement.Business.Domain.OrderServiceSection.Responses;
using OrderManagement.Business.Domain.OrderStateMachineSection;
using OrderManagement.Exceptions;

namespace OrderManagement.Api.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderStateMachineFactory _orderStateMachineFactory;
        private readonly IOrderService _orderService;

        public OrderController(IOrderStateMachineFactory orderStateMachineFactory, IOrderService orderService)
        {
            _orderStateMachineFactory = orderStateMachineFactory;
            _orderService = orderService;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> PostOrder([FromBody] PostOrderRequest postOrderRequest)
        {
            if (postOrderRequest == null)
                throw new RequestNullException();

            IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.CreateOrderStateMachineAsync(postOrderRequest.BuyerName, postOrderRequest.BuyerAddress, postOrderRequest.TotalAmount);
            orderStateMachine.SubmitOrder();

            return StatusCode((int) HttpStatusCode.Created, orderStateMachine.OrderResponse);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrdersRequest getOrdersRequest)
        {
            QueryOrderRequest queryOrderRequest = getOrdersRequest != null
                                                      ? new QueryOrderRequest(getOrdersRequest.Offset,
                                                                              getOrdersRequest.Take
                                                                             )
                                                        {
                                                            OrderId = getOrdersRequest.OrderId
                                                        }
                                                      : null;

            OrderCollectionResponse orderCollectionResponse = await _orderService.QueryOrderAsync(queryOrderRequest);

            return StatusCode((int) HttpStatusCode.OK, orderCollectionResponse);
        }
    }
}