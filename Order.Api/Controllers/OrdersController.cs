using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Models;
using Order.Api.Services;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ServiceBusPublisher _publisher;
        private readonly ServiceBusInspector _inspector;

        public OrdersController(ServiceBusPublisher publisher, ServiceBusInspector inspector)
        {
            _publisher = publisher;
            _inspector = inspector;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderMessage order)
        {
            order.OrderId = Guid.NewGuid();
            await _publisher.PublishOrder(order);

            return Ok($"Order {order.OrderId} published to Service Bus");
        }

        [HttpGet("peek/queue")]
        public async Task<IActionResult> PeekQueue(int max = 10)
        {
            var msgs = await _inspector.PeekQueueMessagesAsync(max);
            return Ok(msgs);
        }

        [HttpGet("peek/topic")]
        public async Task<IActionResult> PeekTopic(string subscription = "orders-sub", int max = 10)
        {
            var msgs = await _inspector.PeekSubscriptionMessagesAsync(subscription, max);
            return Ok(msgs);
        }
    }
}
