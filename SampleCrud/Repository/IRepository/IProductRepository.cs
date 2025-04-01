using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleCrud.Data.Entities;
using SampleCrud.Models;

namespace SampleCrud.Repository.Interfaces
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetAllProducts();

        public Task<Product?> GetProductById(Guid id);

        public Task<List<Product>?> GetProductByName(string name);

        public Task<List<Product>?> GetProductByCategory(string category);

        public Task<List<Product>?> SearchProduct(string keyword);

        public Task<List<Product>?> AddProduct(string username,AddProductDto addProdDto);

        public Task<bool> DeleteProduct(string username,Guid id);

        public Task<Product?> UpdateProduct(string username, Guid id, UpdateProductDto updateProdDto);

        public Task<Product?> PatchProduct(Guid id, [FromBody] JsonPatchDocument<Product> patchDoc);
 

    }
}
