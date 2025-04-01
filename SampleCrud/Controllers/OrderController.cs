using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleCrud.Data.Entities;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;

namespace SampleCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Retrieves the list of orders for the authenticated customer.
        /// </summary>
        /// <returns>A list of orders or a not found result if no orders are found.</returns>
        /// <response code="200">Orders retrieved successfully</response>
        /// <response code="404">No orders found</response>
        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<IActionResult> ViewOrders()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            List<object>? orders = await _orderRepository.ViewOrders(username);
            if (orders is null || !orders.Any())
            {
                return NotFound("No Orders Found.");
            }
            return Ok(orders);
        }

        /// <summary>
        /// Places a new order for the authenticated customer.
        /// </summary>
        /// <param name="addOrderDto">The order details.</param>
        /// <returns>The list of orders after placing the new order or no content if the order could not be placed.</returns>
        /// <response code="200">Order placed successfully</response>
        /// <response code="204">Order could not be placed</response>
        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(AddOrderDto addOrderDto)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            ICollection<object>? orders = await _orderRepository.PlaceOrder(username, addOrderDto);
            if (orders is null)
            {
                return NoContent();
            }
            return Ok(orders);
        }

        /// <summary>
        /// Cancels an existing order for the authenticated customer.
        /// </summary>
        /// <param name="orderid">The ID of the order to cancel.</param>
        /// <returns>A success message or a bad request result if the order could not be canceled.</returns>
        /// <response code="200">Order cancelled successfully</response>
        /// <response code="400">Cannot cancel order</response>
        [Authorize(Roles = "customer")]
        [HttpDelete]
        public async Task<IActionResult> CancelOrder(int orderid)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (!(await _orderRepository.CancelOrder(username, orderid)))
            {
                return BadRequest("Cannot cancel order.");
            }
            return Ok("Order cancelled successfully.");
        }

        /// <summary>
        /// Updates the status of an existing order for the authenticated customer.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="patchDoc">The patch document containing the updates.</param>
        /// <returns>The updated order status or a bad request result if the status could not be updated.</returns>
        /// <response code="200">Order status updated successfully</response>
        /// <response code="400">Can't update order status</response>
        [Authorize(Roles = "customer")]
        [HttpPatch]
        //[Route("{id:int}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, JsonPatchDocument<Order> patchDoc)
        {
            string result = await _orderRepository.UpdateOrderStatus(orderId, patchDoc);
            if (result.Equals(""))
            {
                return BadRequest("Can't update order status");
            }
            return Ok(result);
        }
    }
}
