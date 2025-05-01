using gymvenience_backend.Services;
using gymvenience_backend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Get all orders
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOrders()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Authentication credentials were not found");
            }

            if (IsTokenExpired())
            {
                return Unauthorized("Token has expired. Please reauthenticate.");
            }

            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Get orders by user ID
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized("Authentication credentials were not found");
            }

            if (IsTokenExpired())
            {
                return Unauthorized("Token has expired. Please reauthenticate.");
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);

            if (orders == null || !orders.Any())
                return NotFound("No orders found for this user.");

            return Ok(orders);
        }

        private bool IsTokenExpired()
        {
            var expirationClaim = HttpContext.User.FindFirstValue("exp");
            if (expirationClaim == null)
            {
                return true;
            }

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim));
            var currentTime = DateTimeOffset.UtcNow;

            if (expirationTime <= currentTime)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Mark an order as delivered.
        /// </summary>
        [HttpPost("{orderId}/deliver")]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            var ok = await _orderService.DeliverOrderAsync(orderId);
            if (!ok) return NotFound(new { message = "Order not found or already delivered." });
            return Ok(new { message = "Order marked as delivered." });
        }
    }
}
