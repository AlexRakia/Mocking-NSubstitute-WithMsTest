using UserManagement.Models;

namespace UserManagement.Services
{
    public interface IUserService
    {
        User? GetUser(int id);
        User? GetUserByEmail(string email);
        bool SaveUser(User user);
        bool DeleteUser(int id);
        IEnumerable<User> GetActiveUsers();
        bool ValidateUser(User user);
    }
}