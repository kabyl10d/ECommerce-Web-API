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
        /// Adds an item to the cart.
        /// </summary>
        /// <param name="pid">Product ID</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <returns>Action result with the added cart item</returns>
        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid pid, int quantity)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Customer isn't authorized.");
            }
            CartItem? result = await _cartRepository.AddToCart(username, pid, quantity);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        /// <summary>
        /// Views the cart items.
        /// </summary>
        /// <returns>Action result with the list of cart items</returns>
        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Customer isn't authorized.");
            }
            List<object>? result = await _cartRepository.ViewCart(username);
            if (result is null || !result.Any())
            {
                return NotFound("No Items Found.");
            }
            return Ok(result);
        }

        /// <summary>
        /// Removes an item from the cart.
        /// </summary>
        /// <param name="itemid">Item ID</param>
        /// <returns>Action result indicating the removal status</returns>
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
        /// Patches a cart item.
        /// </summary>
        /// <param name="itemid">Item ID</param>
        /// <param name="patchDoc">Patch document</param>
        /// <returns>Action result indicating the patch status</returns>
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
