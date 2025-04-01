using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SampleCrud.Data;
using SampleCrud.Data.Entities;
using SampleCrud.Exceptions;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SampleCrud.Repository.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _dbContext;

        public UsersRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<List<User>?> GetAllUsers()
        {
              return await _dbContext.Users.ToListAsync();
        }

        public async Task<List<Product>?> GetProductsByMerchant(string username)
        {
            Merchant? merchant = await _dbContext.Merchants.FirstOrDefaultAsync(m => m.MerchantName == username);
            try
            {
                if (merchant == null)
                {
                    throw new UserNotFoundException("Merchant not found.");
                }
                return await _dbContext.Products.Where(p => p.MerchantId == merchant.MerchantId).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

       
        public async Task<bool> AddUser(AddUserDto addUserDto)
        {
            try
            {
                if (_dbContext.Users.Any(u => u.Username == addUserDto.Username || u.Mailid==addUserDto.Mailid && addUserDto.Role == "merchant"))
                {
                    throw new DuplicateUserException("Merchant already exists.");
                }
                else if (_dbContext.Users.Any(u => u.Username == addUserDto.Username || u.Mailid == addUserDto.Mailid && addUserDto.Role == "customer"))
                {
                    throw new DuplicateUserException("Customer already exists.");
                }
                if(addUserDto.Role != "merchant" && addUserDto.Role != "customer")
                {
                    throw new InvalidUserInfoException("Invalid role.");
                }
                var email = new System.Net.Mail.MailAddress(addUserDto.Mailid);
                if (addUserDto.Mailid != email.Address)
                {
                    throw new InvalidUserInfoException("Invalid mailid.");
                }
                if(addUserDto.Phone.Length != 10)
                {
                    throw new InvalidUserInfoException("Invalid phone number.");
                }
                string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$";
                Regex r = new Regex(pattern);
                if (!r.IsMatch(addUserDto.Password))
                {
                    throw new InvalidUserInfoException("At least 8 characters, including at least one " +
                        "uppercase letter, one lowercase letter, one digit, and one special character, with no spaces");
                }
                User UserEntity = new User()
                {
                    Username = addUserDto.Username,
                    Mailid = addUserDto.Mailid,
                    Password = addUserDto.Password,
                    Phone = addUserDto.Phone,
                    Role = addUserDto.Role
                };
                _dbContext.Users.Add(UserEntity);
                await _dbContext.SaveChangesAsync();


                if (UserEntity.Role.Equals("merchant"))
                {
                    Merchant merchant = new Merchant()
                    {
                        MerchantId = UserEntity.UserId,
                        MerchantName = UserEntity.Username
                    };
                    _dbContext.Merchants.Add(merchant);
                    await _dbContext.SaveChangesAsync();
                }
                else if (UserEntity.Role.Equals("customer"))
                {
                    Customer customer = new Customer()
                    {
                        CustomerId = UserEntity.UserId,
                        CustomerName = UserEntity.Username

                    };
                    _dbContext.Customers.Add(customer);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        
        public async Task<bool> DeleteUser(string username)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    throw new UserNotFoundException();
                }

                if (user.Role == "merchant")
                {
                    var merchant = await _dbContext.Merchants.FirstOrDefaultAsync(m => m.MerchantName == username);
                    if (merchant != null)
                    {
                        var products = await _dbContext.Products.Where(p => p.MerchantId == merchant.MerchantId).ToListAsync();
                        _dbContext.Products.RemoveRange(products);
                        _dbContext.Merchants.Remove(merchant);
                    }
                }
                else if (user.Role == "customer")
                {
                    var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerName == username);
                    if (customer != null)
                    {
                        var orders = await _dbContext.Order.Where(o => o.CustomerId == customer.CustomerId).ToListAsync();
                        var cartItems = await _dbContext.Cart.Where(ci => ci.CustomerId == customer.CustomerId).ToListAsync();
                        _dbContext.Order.RemoveRange(orders);
                        _dbContext.Cart.RemoveRange(cartItems);
                        _dbContext.Customers.Remove(customer);
                    }
                }

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<User?> Login(string username, string password)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => (u.Username == username || u.Mailid==username) && u.Password == password);
                if (user == null)
                {
                    throw new UserNotFoundException("Invalid username or password.");
                }
                return user;
            }
            catch
            {
                throw;
            }
        }

        

    }
}
