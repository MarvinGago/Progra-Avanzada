using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;
using Repository.Repositories;

namespace Business;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private const string RestrictedDomain = "@recetashub.com";
    private const int DefaultRoleId = 2; // Estándar

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new BusinessException("Usuario y contraseña son obligatorios.");

        // Bloqueo del dominio admin en registro público
        if (username.Contains(RestrictedDomain, StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("No se puede registrar con ese dominio. Contacte a soporte.");

        bool exists = await _userRepository.UsernameExistsAsync(username);
        if (exists)
            throw new BusinessException("El nombre de usuario ya está en uso.");

        string hash = PasswordHasher.Hash(password);
        var (resultCode, message) = await _userRepository.RegisterUserAsync(username, hash, DefaultRoleId);

        if (resultCode != 1)
            throw new BusinessException(message);

        return message;
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new BusinessException("Usuario y contraseña son obligatorios.");

        var user = await _userRepository.LoginAsync(username);

        if (user == null)
            throw new BusinessException("Usuario o contraseña incorrectos.");

        bool isValid = PasswordHasher.Verify(password, user.PasswordHash);
        if (!isValid)
            throw new BusinessException("Usuario o contraseña incorrectos.");

        return user;
    }
}