using Microsoft.AspNetCore.Mvc;
using gymvenience_backend.Services;
using gymvenience_backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _cartService.GetOrCreateCartForUserAsync(userId);
            return Ok(cart);
        }

        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddProductToCart(string userId, [FromBody] AddProductDto request)
        {
            var cart = await _cartService.AddProductToCartAsync(userId, request.ProductId, request.Quantity);
            return Ok(cart);
        }

        [HttpPost("{userId}/remove")]
        public async Task<IActionResult> RemoveProductFromCart(string userId, [FromBody] RemoveProductDto request)
        {
            var cart = await _cartService.RemoveProductFromCartAsync(userId, request.ProductId);
            return Ok(cart);
        }

        [HttpDelete("{userId}/delete")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            var success = await _cartService.ClearCartByUserIdAsync(userId);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("checkout/{userId}")]
        [Authorize]
        public async Task<IActionResult> Checkout(string userId)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null || jwtToken.ValidTo < System.DateTime.UtcNow)
            {
                return Unauthorized("JWT token is expired or invalid");
            }

            var order = await _cartService.CheckoutAsync(userId);

            if (order == null) return BadRequest("Checkout failed.");

            return Ok(order);
        }
    }
}

