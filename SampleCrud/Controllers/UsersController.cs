using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SampleCrud.Data.Entities;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;

namespace SampleCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IUsersRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets all users. Only accessible by admin.
        /// </summary>
        /// <returns>List of users</returns>
        /// <response></response>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            List<User>? users = await _userRepository.GetAllUsers();
            if (users is null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        /// <summary>
        /// Gets products by merchant. Only accessible by merchant.
        /// </summary>
        /// <returns>List of products</returns>
        [Authorize(Roles = "merchant")]
        [HttpGet]
        [Route("merchant-products")]
        public async Task<IActionResult> GetProductsByMerchant()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Merchant isn't authorized.");
            }

            List<Product>? prods = await _userRepository.GetProductsByMerchant(username);

            if (prods is null || !prods.Any())
            {
                return NotFound("No products found.");
            }

            var ModProd = prods
                .Select(c => new
                {
                    c.ProductId,
                    c.Name,
                    c.MerchantId,
                    c.Merchant.MerchantName,
                    c.Price,
                    c.Category

                }).ToList();

            return Ok(ModProd);
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="addUserDto">User data transfer object</param>
        /// <returns>Result of the operation</returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserDto addUserDto)
        {
            bool isAdded = await _userRepository.AddUser(addUserDto);
            if (!isAdded)
            {
                return BadRequest("User not added.");
            }
            return Ok("User added successfully.");
        }

        /// <summary>
        /// Deletes a user. Only accessible by authorized users.
        /// </summary>
        /// <returns>Result of the operation</returns>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User isn't authorized.");
            }
            bool isDeleted = await _userRepository.DeleteUser(username);
            if (!isDeleted)
            {
                return BadRequest("User not deleted.");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"] ?? throw new ArgumentNullException("key")));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiredToken = new JwtSecurityToken(
                _configuration["JwtConfig:Issuer"],
                _configuration["JwtConfig:Audience"],
                claims,
                expires: DateTime.UtcNow.AddSeconds(1),
                signingCredentials: signIn
            );
            string expiredTokenValue = new JwtSecurityTokenHandler().WriteToken(expiredToken);

            return Ok(new { Message = "User removed successfully.", Token = expiredTokenValue });
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDto">Login data transfer object</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            User? user = await _userRepository.Login(loginDto.Username, loginDto.Password);
            if (user != null)
            {
                var claims = new[]
                {
                                new Claim(JwtRegisteredClaimNames.Sub, _configuration["JwtConfig:Subject"] ?? throw new ArgumentNullException("subject")),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(ClaimTypes.Name, user.Username),
                                new Claim("Email", user.Mailid),
                                new Claim(ClaimTypes.Role, user.Role)
                            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"] ?? throw new ArgumentNullException("key")));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JwtConfig:Issuer"],
                    _configuration["JwtConfig:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signIn
                );
                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { Token = tokenValue, User = user });
            }
            return NoContent();
        }
    }
}
