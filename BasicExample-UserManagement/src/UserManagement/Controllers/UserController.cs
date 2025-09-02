using UserManagement.Models;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public string GetUserDisplayName(int userId)
        {
            var user = _userService.GetUser(userId);
            return user?.Name ?? "Unknown User";
        }

        public bool CreateUser(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                return false;

            var user = new User
            {
                Name = name,
                Email = email,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            if (!_userService.ValidateUser(user))
                return false;

            return _userService.SaveUser(user);
        }

        public bool UpdateUser(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name))
                return false;

            if (!_userService.ValidateUser(user))
                return false;
            
            return _userService.SaveUser(user);
        }

        public bool DeactivateUser(int userId)
        {
            var user = _userService.GetUser(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            return _userService.SaveUser(user);
        }

        public IEnumerable<string> GetActiveUserNames()
        {
            return _userService.GetActiveUsers().Select(u => u.Name);
        }

        public User? FindUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return _userService.GetUserByEmail(email);
        }
    }
}