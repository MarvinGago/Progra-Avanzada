using Models.Models;

namespace Business;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<string> CreateUserAsync(string username, string password, int roleId);
    Task ChangeRoleAsync(int userId, int newRoleId);
    Task DeleteUserAsync(int userId);
}