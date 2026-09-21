using Microsoft.EntityFrameworkCore;
using onlineStore.Data;
using onlineStore.Models;
using onlineStore.Services.Interfaces;
namespace onlineStore.Services.Implemntations
{
    public class AccountService : IAccountService
    {
        private readonly OnlineStoreDbContext _context;

        public AccountService(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            if (user.IsBlocked)
                return user;

            if (!user.IsApproved)
                return user;

            return user;
        }

        public async Task<bool> RegisterAsync(string fullName, string email, string password)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email);

            if (exists)
                return false;

            var userRole = await _context.Roles.FirstAsync(r => r.Name == "User");

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsApproved = false,
                IsBlocked = false,
                RoleId = userRole.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
