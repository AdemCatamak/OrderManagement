using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Contracts.OrderRequests;
using OrderManagement.Business.OrderServiceSection;
using OrderManagement.Business.OrderServiceSection.Requests;
using OrderManagement.Business.OrderServiceSection.Responses;

namespace OrderManagement.Api.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> PostOrder([FromBody] PostOrderRequest postOrderRequest)
        {
            CreateOrderRequest createOrderRequest = postOrderRequest != null
                                                        ? new CreateOrderRequest(postOrderRequest.BuyerName,
                                                                                 postOrderRequest.BuyerAddress,
                                                                                 postOrderRequest.TotalAmount)
                                                        : null;

            OrderResponse orderResponse = await _orderService.CreateOrderAsync(createOrderRequest);

            return StatusCode((int) HttpStatusCode.Created, orderResponse);
        }
    }
}