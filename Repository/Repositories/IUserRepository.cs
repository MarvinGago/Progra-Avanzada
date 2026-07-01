using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;


namespace Repository.Repositories;


public interface IUserRepository
{
    Task<bool> UsernameExistsAsync(string username);
    Task<(int ResultCode, string Message)> RegisterUserAsync(string username, string passwordHash, int roleId);
    Task<User?> LoginAsync(string username);
    Task<IEnumerable<Models.Models.User>> GetAllUsersAsync();
    Task ChangeRoleAsync(int userId, int newRoleId);
    Task DeleteUserAsync(int userId);
}
