using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SampleCrud.Data;
using SampleCrud.Data.Entities;
using SampleCrud.Exceptions;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Repository.Implementations
{

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        async Task OrderSaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<object>?> ViewOrders(string username)
        {
            var customer = await _dbContext.Customers
                                .Include(u => u.Orders)
                                .FirstOrDefaultAsync(u => u.CustomerName == username);

            var userOrders = await _dbContext.Order.Where(c => c.CustomerId == customer.CustomerId)
                    .Select(c => new
                    {
                        c.OrderId, 
                        c.CustomerId,
                        c.ProductId,
                        c.TotalAmount,
                        c.PaymentMethod,
                        c.OrderDate,
                        c.ShippingAddress,
                        c.Status,
                    }).ToListAsync();
            return userOrders.Cast<object>().ToList();
        }

        public async Task<List<object>?> PlaceOrder(string username, AddOrderDto addOrderDto)
        {
            Customer? user = await _dbContext.Customers
                                .Include(u => u.Orders)
                                .FirstOrDefaultAsync(u => u.CustomerName == username);
            try
            {
                if (user == null)
                {
                    throw new UserNotFoundException();
                }
                List<CartItem> items = await _dbContext.Cart.Where(p => p.Customer.CustomerName == username).ToListAsync();
                if (items.Count == 0)
                {
                    throw new CartItemNotFoundException("Cart is empty.");
                }

                double total = 0;

                foreach (var item in items)
                {
                    total += item.Quantity * (await _dbContext.Products.FindAsync(item.ProductId))!.Price;
                }
                foreach (var item in items)
                {
                    Order order = new Order()
                    {
                        CustomerId = item.CustomerId,
                        ProductId = item.ProductId,
                        TotalAmount = item.Quantity * (await _dbContext.Products.FindAsync(item.ProductId))!.Price, 
                        PaymentMethod = addOrderDto.PaymentMethod,
                        OrderDate = DateTime.Now,
                        Quantity = item.Quantity,
                        ShippingAddress = addOrderDto.ShippingAddress,
                        Status = OrderStatus.Processing
                    };

                    await _dbContext.Order.AddAsync(order);
                    _dbContext.Cart.Remove(item);
                }
                await OrderSaveChangesAsync();
                var userOrders = await _dbContext.Order.Where(c => c.CustomerId == user.CustomerId)
                    .Select(c => new
                    { 
                        c.OrderId,
                        c.CustomerId,
                        c.ProductId,
                        c.TotalAmount,
                        c.PaymentMethod,
                        c.OrderDate,
                        c.ShippingAddress,
                        c.Status,
                    }).ToListAsync();
                return userOrders.Cast<object>().ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CancelOrder(string username, int orderid)
        {
            Customer? user = await _dbContext.Customers
                                .Include(u => u.Orders)
                                .FirstOrDefaultAsync(u => u.CustomerName == username);
            try
            {
                if (user is null)
                {
                    throw new UserNotFoundException();
                }
                var order = await _dbContext.Order.SingleOrDefaultAsync(p => p.Customer.CustomerName == username && p.OrderId == orderid);

                if (order == null || order.Status == OrderStatus.Delivered)
                {
                    return false;
                }

                (await _dbContext.Products.FindAsync(order.ProductId))!.Stock += order.Quantity;
                _dbContext.Order.Remove(order);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        public async Task<string> UpdateOrderStatus(int orderId, JsonPatchDocument<Order> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    throw new PatchDocumentNotFoundException();
                }
                var item = await _dbContext.Order.FindAsync(orderId);

                if (item == null)
                {
                    throw new OrderNotFoundException();
                }

                patchDoc.ApplyTo(item);
                await OrderSaveChangesAsync();
                return $"Status updated to {item.Status}";
            }
            catch
            {
                throw;
            }
        }
    }
}
