using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleCrud.Data.Entities;
using SampleCrud.Repository.Interfaces;

namespace SampleCrud.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        /// <summary>
        /// Add a specific product to cart
        /// </summary>
        /// <param name="pid">Product ID</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <returns>Action result indicating the outcome of the operation</returns>
        /// <response code="200">Product added to cart</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't add to cart right now</response> 
        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid pid, int quantity)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;

            CartItem? result = await _cartRepository.AddToCart(username, pid, quantity);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        /// <summary>
        /// View the items in the cart
        /// </summary>
        /// <returns>Action result containing the list of items in the cart</returns>
        /// <response code="200">Cart items retrieved successfully</response>
        /// <response code="400">Failed to retrieve cart items</response>
        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;

            List<object>? result = await _cartRepository.ViewCart(username);
            if (result is null || !result.Any())
            {
                return BadRequest();
            }
            return Ok(result);
        }

        /// <summary>
        /// Remove a specific item from the cart
        /// </summary>
        /// <param name="itemid">Item ID</param>
        /// <returns>Action result indicating the outcome of the operation</returns>
        /// <response code="200">Item removed from cart</response>
        /// <response code="404">Item not found in cart</response>
        [Authorize(Roles = "customer")]
        [HttpDelete]
        public async Task<IActionResult> RemoveFromCart(int itemid)
        {
            bool result = await _cartRepository.RemoveFromcart(itemid);
            if (!result)
            {
                return NotFound();
            }
            return Ok("Removed from cart");
        }

        /// <summary>
        /// Update a specific item in the cart
        /// </summary>
        /// <param name="itemid">Item ID</param>
        /// <param name="patchDoc">Patch document containing the updates</param>
        /// <returns>Action result indicating the outcome of the operation</returns>
        /// <response code="200">Item updated successfully</response>
        /// <response code="400">Failed to update item</response>
        [Authorize(Roles = "customer")]
        [HttpPatch]
        public async Task<IActionResult> PatchCartItem(int itemid, JsonPatchDocument<CartItem> patchDoc)
        {
            bool result = await _cartRepository.PatchCartItem(itemid, patchDoc);
            if (result is false)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
