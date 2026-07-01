using System.Security.Claims;
using Business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/account/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {
        try
        {
            var message = await _authService.RegisterAsync(request.Username, request.Password);
            return Ok(new { success = true, message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // POST api/account/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _authService.LoginAsync(request.Username, request.Password);

            // Crear los claims del usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Estándar")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return Ok(new { success = true, message = "Login exitoso.", role = user.Role?.RoleName });
        }
        catch (BusinessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
    }

    // POST api/account/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return Ok(new { success = true, message = "Sesión cerrada." });
    }
}

// DTO de entrada para Login y Register
public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
