using onlineStore.Models;

namespace onlineStore.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string fullName, string email, string password);
    }
}
