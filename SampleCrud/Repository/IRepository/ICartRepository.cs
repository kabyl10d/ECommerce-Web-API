using Microsoft.AspNetCore.JsonPatch;
using SampleCrud.Data.Entities;

namespace SampleCrud.Repository.Interfaces
{
    public interface ICartRepository
    {
        public Task<CartItem?> AddToCart(string username, Guid pid, int quantity);

        public Task<List<object>?> ViewCart(string username);

        public Task<bool> RemoveFromcart(int itemid);

        public Task<bool> PatchCartItem(int itemid, JsonPatchDocument<CartItem> patchDoc);
    }
}
