using Models.Models;
using Repository.Repositories;

namespace Business;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private const string AdminDomain = "@recetashub.com";

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<string> CreateUserAsync(string username, string password, int roleId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new BusinessException("Usuario y contraseña son obligatorios.");

        // Si el rol es Admin, el username debe tener el dominio
        if (roleId == 1 && !username.Contains(AdminDomain, StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("Los administradores deben usar el dominio @@recetashub.com.");

        // Si el rol es Estándar, el username NO debe tener el dominio
        if (roleId == 2 && username.Contains(AdminDomain, StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("Los usuarios estándar no pueden usar el dominio @@recetashub.com.");

        bool exists = await _userRepository.UsernameExistsAsync(username);
        if (exists)
            throw new BusinessException("El nombre de usuario ya está en uso.");

        string hash = PasswordHasher.Hash(password);
        var (resultCode, message) = await _userRepository.RegisterUserAsync(username, hash, roleId);

        if (resultCode != 1)
            throw new BusinessException(message);

        return message;
    }

    public async Task ChangeRoleAsync(int userId, int newRoleId)
    {
        await _userRepository.ChangeRoleAsync(userId, newRoleId);
    }

    public async Task DeleteUserAsync(int userId)
    {
        await _userRepository.DeleteUserAsync(userId);
    }
}