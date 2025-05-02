using gymvenience_backend.DTOs;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService      _orderService;
        private readonly IProductRepository _productRepo;

        public PaymentController(
            IOrderService orderService,
            IProductRepository productRepo)
        {
            _orderService  = orderService;
            _productRepo   = productRepo;
        }

        // 1️⃣ Create the Stripe Checkout Session
        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession(
            [FromBody] CheckoutSessionRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found.");

            var lineItems = request.Items.Select(i => new SessionLineItemOptions
            {
                Quantity  = i.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency    = "eur",
                    UnitAmount  = (long)Math.Round(i.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name     = $"Item-{i.ProductId}",
                        Metadata = new Dictionary<string,string>
                        {
                            { "productId", i.ProductId }
                        }
                    }
                }
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems          = lineItems,
                Mode               = "payment",
                SuccessUrl         = "http://localhost:5173/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl          = "http://localhost:5173/cancel",
                Metadata           = new Dictionary<string,string>
                {
                    { "userId",     userId },
                    { "shipping",   request.ShippingMethod },
                    { "totalPrice", request.TotalPrice.ToString("F2") }
                }
            };

            var session = await new SessionService().CreateAsync(options);
            return Ok(new { sessionId = session.Id });
        }

        // 2️⃣ Complete the order exactly once
        [HttpPost("complete-order")]
        [Authorize]
        public async Task<IActionResult> CompleteOrder(
            [FromBody] CompleteOrderRequest req)
        {
            var sessService = new SessionService();
            var session = await sessService.GetAsync(req.SessionId, new SessionGetOptions
            {
                Expand = new List<string> { "payment_intent" }
            });

            if (session == null || session.PaymentStatus != "paid")
                return BadRequest("Payment not completed.");

            // idempotency guard
            if (await _orderService.ExistsForSessionAsync(session.Id))
                return Ok(new { message = "Order already recorded." });

            // fetch line items
            var lineItems = await sessService.ListLineItemsAsync(
                session.Id,
                new SessionLineItemListOptions
                {
                    Limit  = 100,
                    Expand = new List<string> { "data.price.product" }
                });

            // build order
            var order = new Order
            {
                StripeSessionId = session.Id,
                UserId          = User.FindFirstValue(ClaimTypes.NameIdentifier),
                OrderDate       = DateTime.UtcNow,
                Items           = new List<OrderItem>()
            };

            foreach (var li in lineItems.Data)
            {
                var productMeta = li.Price.Product.Metadata;
                var productId   = productMeta["productId"];
                var qty         = li.Quantity ?? 1;
                var price       = li.AmountSubtotal / 100.0;

                order.Items.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity  = (int)qty,
                    Price     = price
                });

                // reduce stock
                var p = await _productRepo.GetByIdAsync(productId);
                if (p != null)
                {
                    p.Quantity = (int)Math.Max(0, p.Quantity - qty);
                    await _productRepo.UpdateAsync(p);
                }
            }

            await _orderService.CreateOrderAsync(order);
            return Ok(new { message = "Order stored." });
        }

        // 3️⃣ Lookup the saved order — no side-effects
        [HttpGet("session/{sessionId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderBySession(string sessionId)
        {
            var order = await _orderService.GetBySessionAsync(sessionId);
            if (order == null)
                return NotFound(new { error = "Order not found." });

            return Ok(new { orderId = order.Id });
        }
    }
}
