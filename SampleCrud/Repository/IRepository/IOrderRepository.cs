using Microsoft.AspNetCore.JsonPatch;
using SampleCrud.Data.Entities;
using SampleCrud.Models;

namespace SampleCrud.Repository.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<object>?> ViewOrders(string username);
        public Task<List<object>?> PlaceOrder(string username, AddOrderDto addOrderDto);
        public Task<bool> CancelOrder(string username, int orderid);

        public Task<string> UpdateOrderStatus(int orderId, JsonPatchDocument<Order> patchDoc);
    }
}
