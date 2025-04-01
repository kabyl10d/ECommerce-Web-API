using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SampleCrud.Data;
using SampleCrud.Data.Entities;
using SampleCrud.Exceptions;
using SampleCrud.Repository.Interfaces;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Repository.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _dbContext;

        public CartRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        async Task CartSaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CartItem?> AddToCart(string username, Guid pid, int quantity)
        {
            Product? product = await _dbContext.Products.FindAsync(pid);
            Customer? user = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerName == username);

            CartItem cartItem = new CartItem();

            try
            {
                if (user == null)
                {
                    throw new UserNotFoundException();
                }
                if (product == null)
                {
                    throw new ProductNotFoundException();
                }

                cartItem = new CartItem
                {
                    CustomerId = user.CustomerId,
                    ProductId = pid,
                    Quantity = quantity,
                    Status = OrderStatus.Processing
                };
                product.Stock -= quantity;

                _dbContext.Cart.Add(cartItem);

                await CartSaveChanges();

                return cartItem;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<object>?> ViewCart(string username)
        {
            Customer? user = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerName == username);
            try
            {
                if (user == null)
                {
                    throw new UserNotFoundException();
                }

                var userCart = await _dbContext.Cart.Where(c => c.CustomerId == user.CustomerId)
                    .Select(c => new
                    {
                        c.ItemId,
                        c.CustomerId,
                        c.ProductId,
                        c.Product.Name,
                        c.Product.Price,
                        c.Quantity,
                        c.Status,
                    }).ToListAsync();

                return userCart.Cast<object>().ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RemoveFromcart(int itemid)
        {
            CartItem? item = await _dbContext.Cart.FirstOrDefaultAsync(c => c.ItemId == itemid);
            try
            {
                if (item == null)
                {
                    throw new CartItemNotFoundException();
                }

                Product? product = await _dbContext.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }

                _dbContext.Cart.Remove(item);
                await CartSaveChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }
         
        public async Task<bool> PatchCartItem(int itemid, JsonPatchDocument<CartItem> patchDoc)
        {
            
            try
            {
                if (patchDoc == null)
                {
                    throw new PatchDocumentNotFoundException();
                }

                var item = await _dbContext.Cart.FindAsync(itemid);

                if (item == null)
                {
                    throw new CartItemNotFoundException();
                }

                foreach (var operation in patchDoc.Operations)
                {
                    if (operation.op == "replace" && operation.path == "/Quantity")
                    {
                        int newQuantity = (int)operation.value;
                        var product = await _dbContext.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            if (newQuantity > item.Quantity)
                            {
                                product.Stock -= (newQuantity - item.Quantity);
                            }
                            else if (newQuantity < item.Quantity)
                            {
                                product.Stock += (item.Quantity - newQuantity);
                            }
                        }
                    }
                }
                
                patchDoc.ApplyTo(item);
                await CartSaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}

