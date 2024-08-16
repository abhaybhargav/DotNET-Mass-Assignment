using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VulnerableAPI.Models;

namespace VulnerableAPI.Services
{
    public interface IUserService
    {
        User? GetUser(string email);
        bool AddUser(User user);
        string GenerateJwtToken(User user);
    }

    public class UserService : IUserService
    {
        private readonly ConcurrentDictionary<string, User> _users = new();
        private readonly string _jwtKey;

        public UserService(IConfiguration configuration)
        {
            _jwtKey = configuration["Jwt:Key"] ?? "your-super-secret-key-that-should-be-kept-safe";
        }

        public User? GetUser(string email)
        {
            _users.TryGetValue(email, out var user);
            return user;
        }

        public bool AddUser(User user)
        {
            return _users.TryAdd(user.Email, user);
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "VulnerableAPI",
                Audience = "VulnerableAPI",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}