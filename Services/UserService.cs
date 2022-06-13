using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoAPI.Helpers;
using TodoAPI.Models;
namespace TodoAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(long id);
        string Authenticate(string username, string password);
        Task<IActionResult> Register(User user);
        Task<IActionResult> UpdateById(long id, User user);
    }
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly TodoContext _context;

        public UserService(TodoContext todoContext, IOptions<AppSettings> appSetting)
        {
            _context = todoContext;
            _appSettings = appSetting.Value;
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetById(long id)
        {
            return await _context.Users.FindAsync(id);
        }
        public string Authenticate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.username == username);
            if (user == null || user.password != password)
            {
                return "";
            }


            var token = GenerateTokenJWT(user);

            return token;
        }
        public async Task<IActionResult> Register(User user)
        {

            if (_context.Users == null)
            {
                throw new Exception("Entity set 'TodoContext.Users'  is null.");
            }
            var existedUser = await _context.Users.FirstOrDefaultAsync(u => u.username == user.username);
            if (existedUser != null)
            {
                return new BadRequestObjectResult(new { message = "username is already existed" });
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new CreatedAtActionResult("GetById", "Users", new { id = user.UserID }, user);
        }
        public async Task<IActionResult> UpdateById(long id, User user)
        {
            var existedUser = await _context.Users.FindAsync(id);
            if (existedUser == null)
            {
                return new NotFoundObjectResult(new { message = "User doesn't exist" });
            }
            try
            {
                _context.ChangeTracker.Clear();
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new JsonResult(new { message = "Update successful", user });
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e);
                throw;
            }




        }
        private string GenerateTokenJWT(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!string.IsNullOrEmpty(_appSettings.Secret))
            {
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserID.ToString()),
                    new Claim("name",user.Name)}),
                    Expires = DateTime.UtcNow.AddHours(4),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            return "";
        }
    }
}
