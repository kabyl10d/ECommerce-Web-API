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

        
        [Authorize(Roles = "customer")]
        [HttpGet]
        public async Task<IActionResult> ViewOrders()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Customer isn't authorized.");
            }
            List<object>? orders = await _orderRepository.ViewOrders(username);
            if (orders is null || !orders.Any())
            {
                return NotFound("No Orders Found.");
            }
            return Ok(orders);
        }

         
        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(AddOrderDto addOrderDto)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Customer isn't authorized.");
            }
            ICollection<object>? orders = await _orderRepository.PlaceOrder(username, addOrderDto);
            if (orders is null)
            {
                return NoContent();
            }
            return Ok(orders);
        }

         
        [Authorize(Roles = "customer")]
        [HttpDelete]
        public async Task<IActionResult> CancelOrder(int orderid)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Customer isn't authorized.");
            }
            if (!(await _orderRepository.CancelOrder(username, orderid)))
            {
                return BadRequest("Cannot cancel order.");
            }
            return Ok("Order cancelled successfully.");
        }

        
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
