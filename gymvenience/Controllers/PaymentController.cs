using gymvenience.Models;
using gymvenience.Services.ReservationService;
using gymvenience_backend.DTOs;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.ReservationRepo;
using gymvenience_backend.Repositories.UserRepo;
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
        private readonly IOrderService _orderService;
        private readonly IProductRepository _productRepo;
        private readonly IReservationService _reservationService;
        private readonly IUserRepository _userRepo;
        private readonly IReservationRepository   _reservationRepo; 

        public PaymentController(
            IOrderService orderService,
            IProductRepository productRepo,
            IReservationService reservationService,
            IReservationRepository reservationRepo,
            IUserRepository userRepo)
        {
            _orderService = orderService;
            _productRepo = productRepo;
            _reservationRepo    = reservationRepo;
            _reservationService = reservationService;
            _userRepo = userRepo;
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
                Quantity = i.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = (long)Math.Round(i.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Item-{i.ProductId}",
                        Metadata = new Dictionary<string, string>
                        {
                            { "productId", i.ProductId }
                        }
                    }
                }
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "http://localhost:5173/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:5173/cancel",
                Metadata = new Dictionary<string, string>
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
                    Limit = 100,
                    Expand = new List<string> { "data.price.product" }
                });

            // build order
            var order = new Order
            {
                StripeSessionId = session.Id,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            foreach (var li in lineItems.Data)
            {
                var productMeta = li.Price.Product.Metadata;
                var productId = productMeta["productId"];
                var qty = li.Quantity ?? 1;
                var price = li.AmountSubtotal / 100.0;

                order.Items.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity = (int)qty,
                    Price = price
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
           [HttpPost("create-reservation-session")]
    [Authorize]
    public async Task<IActionResult> CreateReservationSession([FromBody] ReservationSessionRequest req)
    {
        var bookerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(bookerId))
            return Unauthorized("User ID not found.");

        var trainer = await _userRepo.GetByIdAsync(req.TrainerId);
        if (trainer == null || !trainer.IsTrainer)
            return BadRequest("Trainer not found.");

        var parts    = req.Duration.Split(':');
        var totalMin = int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
        var hours    = totalMin / 60m;
        var price    = trainer.HourlyRate * hours;

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode               = "payment",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency   = "eur",
                        UnitAmount = (long)Math.Round(price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Rezervacija su {trainer.Name} {trainer.Surname}"
                        }
                    }
                }
            },
            SuccessUrl = $"{req.Origin}/reservation-success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl  = $"{req.Origin}/reservation-cancel",
            Metadata = new Dictionary<string,string>
            {
                { "bookerId",  bookerId        },
                { "trainerId", req.TrainerId   },
                { "slotId",    req.SlotId      },  // ← include slotId!
                { "date",      req.Date        },
                { "startTime", req.StartTime   },
                { "duration",  req.Duration    }
            }
        };

        var session = await new SessionService().CreateAsync(options);
        return Ok(new { sessionId = session.Id, url = session.Url });
    }

        [HttpPost("complete-reservation")]
        [Authorize]
        public async Task<IActionResult> CompleteReservation([FromBody] CompleteReservationRequest req)
        {
            try
            {
                var svc = new SessionService();
                var session = await svc.GetAsync(req.SessionId, new SessionGetOptions
                {
                    Expand = new List<string> { "payment_intent" }
                });

                if (session == null || session.PaymentStatus != "paid")
                    return BadRequest(new { error = "Payment not completed." });

                if (await _reservationService.ExistsForSessionAsync(session.Id))
                    return Ok(new { message = "Reservation already exists." });

                // 1) Pull metadata
                var m = session.Metadata;
                if (!m.TryGetValue("bookerId", out var bookerId) ||
                    !m.TryGetValue("trainerId", out var trainerId) ||
                    !m.TryGetValue("slotId", out var slotId) ||
                    !m.TryGetValue("date", out var dateStr) ||
                    !m.TryGetValue("startTime", out var startStr) ||
                    !m.TryGetValue("duration", out var durStr))
                {
                    return BadRequest(new { error = "Missing reservation metadata." });
                }

                // 2) Atomically mark the slot reserved
                var slot = _reservationRepo.GetAvailableTimeSlot(slotId);
                if (slot == null)
                    return BadRequest(new { error = "Time slot not available." });

                slot.Reserved = true;
                _reservationRepo.MarkTimeSlotReserved(slot);
                await _reservationRepo.SaveChangesAsync();

                // 3) Parse date/time/duration
                var date = DateTime.Parse(dateStr);
                var start = TimeSpan.Parse(startStr);
                var duration = TimeSpan.Parse(durStr);

                // 4) Re-fetch trainer for gym/rate
                var trainer = await _userRepo.GetByIdAsync(trainerId);
                if (trainer == null || !trainer.IsTrainer)
                    return BadRequest(new { error = "Trainer not found." });

                // 5) Create and save Reservation
                var reservation = new Reservation
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = bookerId,
                    TrainerId = trainerId,
                    Date = date,
                    Time = start,
                    Duration = duration,
                    GymId = trainer.Gym?.Id ?? string.Empty,
                    RateAtBooking = trainer.HourlyRate,
                    IsDone = false,
                    StripeSessionId = session.Id
                };

                await _reservationService.CreateReservationAsync(reservation);
                return Ok(new { message = "Reservation confirmed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }     /// <summary>
            /// 3️⃣ Lookup a saved Reservation by its Stripe session ID.
            /// </summary>
            [HttpGet("reservation/{sessionId}")]
            [Authorize]
            public async Task<IActionResult> GetReservationBySession(string sessionId)
            {
                var res = await _reservationService.GetBySessionAsync(sessionId);
                if (res == null)
                    return NotFound(new { error = "Reservation not found." });
                return Ok(res);
            }

        }
    }

