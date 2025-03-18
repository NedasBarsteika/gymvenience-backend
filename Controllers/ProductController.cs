using AutoMapper;
using gymvenience_backend.Common;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.ProductService;
using gymvenience_backend.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public ProductController(IProductRepository productRepository, IMapper mapper, IUserService userService, IProductService bookService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _userService = userService;
            _productService = bookService;
        }

        [HttpGet("", Name = "GetProductList")]
        public async Task<IEnumerable<ProductListView>> GetProductList()
        {
            var allProducts = await _productRepository.GetAllAsync();
            var productList = _mapper.Map<IEnumerable<ProductListView>>(allProducts);

            return productList;
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<ProductListView>> GetProduct(string id)
        {
            var requiredProduct = await _productRepository.GetByIdAsync(id);
            if(requiredProduct == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProductListView>(requiredProduct);
        }

        // Pakeisti su kategorijoms
        [HttpPost("buy", Name = "BuyProducts")]
        [Authorize]
        public async Task<ActionResult> BuyProducts([FromBody] List<string> productIds,
                                            [FromQuery] string category)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Authentication credentials were not found");
            }

            ProductType pCategory = category.ToLower() == "proteinpowder" ? ProductType.ProteinPowder : ProductType.Dumbbells;

            var result = await _userService.AddNewPurchaseAsync(userId, productIds, pCategory);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok();
        }

        [HttpGet("purchases", Name = "GetPurchasedProducts")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProductListView>>> GetPurchasedProducts()
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

            var (result, purchasedUserProducts) = await _userService.GetAllPurchasesAsync(userId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var sanitizedProductList = _mapper.Map<IEnumerable<PurchaseListView>>(purchasedUserProducts);

            return Ok(sanitizedProductList);
        }


        [HttpGet("search", Name = "GetSearchResults")]
        public async Task<IEnumerable<ProductListView>> GetSearchResults(
                    [FromQuery] string? search = null,
                    [FromQuery] string? category = null)
        {
            if (search is null)
            {
                search = "";
            }

            if (category is null)
            {
                category = "any";
            }

            var requiredProducts = await _productService.SearchForProductsAsync(search, category);
            var sanitizedProductList = _mapper.Map<IEnumerable<ProductListView>>(requiredProducts);
            return sanitizedProductList;
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

    }
}
