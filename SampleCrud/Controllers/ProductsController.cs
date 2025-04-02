using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleCrud.Data.Entities;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns>List of all products.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok(await _productRepository.GetAllProducts());
        }

        /// <summary>
        /// Get product by ID.
        /// </summary>
        /// <param name="Productid">Product ID.</param>
        /// <returns>Product details.</returns>
        [Authorize]
        [HttpGet]
        [Route("{Productid:guid}")]
        public async Task<IActionResult> GetProductById(Guid Productid)
        {
            Product? product = await _productRepository.GetProductById(Productid);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// Get product by name.
        /// </summary>
        /// <param name="ProductName">Product name.</param>
        /// <returns>List of products with the specified name.</returns>
        [Authorize]
        [HttpGet]
        [Route("{ProductName}")]
        public async Task<IActionResult> GetProductByName(string ProductName)
        {
            List<Product>? products = await _productRepository.GetProductByName(ProductName);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }

        /// <summary>
        /// Get products by category.
        /// </summary>
        /// <param name="category">Product category.</param>
        /// <returns>List of products in the specified category.</returns>
        [Authorize]
        [HttpGet]
        [Route("Category")]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            List<Product>? products = await _productRepository.GetProductByCategory(category);
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);
        }

        /// <summary>
        /// Search products by keyword.
        /// </summary>
        /// <param name="keyword">Search keyword.</param>
        /// <returns>List of products matching the keyword.</returns>
        [Authorize]
        [HttpGet]
        [Route("Keyword")]
        public async Task<IActionResult> SearchProduct(string keyword)
        {
            List<Product>? products = await _productRepository.SearchProduct(keyword);
            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }

        /// <summary>
        /// Add a new product.
        /// </summary>
        /// <param name="addProdDto">Product details.</param>
        /// <returns>List of products after addition.</returns>
        [Authorize(Roles = "merchant")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDto addProdDto)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Merchant isn't authorized.");
            }
            List<Product>? products = await _productRepository.AddProduct(username, addProdDto);
            if (products == null || products.Count == 0)
            {
                return BadRequest("Product could not be added.");
            }
            return Ok(products);
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="updateProdDto">Updated product details.</param>
        /// <returns>Updated product details.</returns>
        [Authorize(Roles = "merchant")]
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto updateProdDto)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Merchant isn't authorized.");
            }
            Product? product = await _productRepository.UpdateProduct(username, id, updateProdDto);
            if (product == null)
            {
                return BadRequest("Product not updated.");
            }
            return Ok(product);
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>Deletion status.</returns>
        [Authorize(Roles = "merchant,admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Merchant isn't authorized.");
            }
            bool delStat = await _productRepository.DeleteProduct(username, id);
            if (!delStat)
            {
                return BadRequest("Couldn't delete product.");
            }
            return Ok("Product deleted.");
        }

        /// <summary>
        /// Patch an existing product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="patchDoc">Patch document.</param>
        /// <returns>Updated product details.</returns>
        [Authorize(Roles = "merchant")]
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PatchProduct(Guid id, [FromBody] JsonPatchDocument<Product> patchDoc)
        {
            Product? product = await _productRepository.PatchProduct(id, patchDoc);
            if (product == null)
            {
                return BadRequest("Product not updated.");
            }
            return Ok(product);
        }
    }
}
