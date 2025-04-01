using SampleCrud.Data.Entities;
using SampleCrud.Models;

namespace SampleCrud.Repository.Interfaces
{
    public interface IUsersRepository
    {
        Task<List<User>?> GetAllUsers();
        Task<List<Product>?> GetProductsByMerchant(string username);
        Task<bool> AddUser(AddUserDto addUserDto);
        Task<bool> DeleteUser(string username);
        Task<User> Login(string username, string password);


    }
}
