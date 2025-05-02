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
        // POST api/products
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDetailedView>> Create([FromBody] CreateProductDto dto)
        {
            var prod = await _productService.CreateProductAsync(dto);
            var view = _mapper.Map<ProductDetailedView>(prod);
            return CreatedAtRoute("GetProduct", new { id = prod.Id }, view);
        }

        // PUT api/products/{id}
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDto dto)
        {
            var ok = await _productService.UpdateProductAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE api/products/{id}
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _productService.DeleteProductAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
