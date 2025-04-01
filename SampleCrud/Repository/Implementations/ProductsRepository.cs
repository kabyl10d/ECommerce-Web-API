using System.Security.Claims;
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

    public class ProductsRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;

        public ProductsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        async Task ProductSaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            Product? product = await _dbContext.Products.FindAsync(id);

            return product;
        }

        public async Task<List<Product>?> GetProductByName(string name)
        {
            List<Product>? products = await _dbContext.Products.Where(p => p.Name == name).ToListAsync();

            return products;
        }

        public async Task<List<Product>?> GetProductByCategory(string category)
        {
            List<Product>? products = await _dbContext.Products.Where(p => p.Category.ToString() == category).ToListAsync();

            return products;
        }

        public async Task<List<Product>?> SearchProduct(string keyword)
        {
            List<Product>? products = await _dbContext.Products.Where(p => p.Name.ToLower().Contains(keyword.ToLower())).ToListAsync();

            return products;
        }

        public async Task<List<Product>?> AddProduct(string username,AddProductDto addProdDto)
        {

            User? merchant = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            try
            {
                if (merchant == null || !await _dbContext.Users.AnyAsync(u => u.Username == merchant.Username))
                {
                    throw new UserNotFoundException("Merchant not found.");
                }

                var product = new Product
                {
                    Name = addProdDto.Name,
                    Price = addProdDto.Price,
                    Stock = addProdDto.Stock,
                    MerchantId = merchant.UserId,
                    Category = Enum.Parse<Category>(addProdDto.Category, true)
                };

                await _dbContext.Products.AddAsync(product);
                await ProductSaveChanges();

                return await _dbContext.Products.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Product?> UpdateProduct(string username, Guid id, UpdateProductDto updateProdDto)
        {

            Product? product = await _dbContext.Products.FindAsync(id);
            try
            {
                if (product == null || !product.Merchant.MerchantName.Equals(username))
                {
                    throw new ProductNotFoundException();
                }

                product.Name = updateProdDto.Name;
                product.Price = updateProdDto.Price;
                product.Stock = updateProdDto.Stock;
                product.Category = Enum.Parse<Category>(updateProdDto.Category, true);
                product.MerchantId = updateProdDto.MerchId;

                await ProductSaveChanges();

                return product;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteProduct(string username, Guid id)
        {
            Product? product = await _dbContext.Products.FindAsync(id);
            try
            {
                if (product == null || product.Merchant.MerchantName!=username || username!="admin")
                {
                    throw new ProductNotFoundException();
                }

                _dbContext.Products.Remove(product);
                await ProductSaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Product?> PatchProduct(Guid id, JsonPatchDocument<Product> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    throw new PatchDocumentNotFoundException();
                }

                var product = await _dbContext.Products.FindAsync(id);

                if (product == null)
                {
                    throw new ProductNotFoundException();
                }

                patchDoc.ApplyTo(product);
                await ProductSaveChanges();

                return product;
            }
            catch
            {
                throw;
            }
        }
    }
}
