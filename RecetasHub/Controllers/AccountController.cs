using System.Security.Claims;
using System.Text.Json;
using Architecture.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecetasHub.Models;

namespace RecetasHub.Controllers;

public class AccountController : Controller
{
    private readonly IRestProvider _restProvider;
    private readonly string _apiBase;

    public AccountController(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _apiBase = configuration["ApiSettings:BaseUrl"]!;
    }

    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(new
            {
                username = model.Username,
                password = model.Password
            });

            // Llamada manual con HttpClient para manejar respuestas no-2xx
            var client = new HttpClient();
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync($"{_apiBase}/api/account/login", content);

            var resultString = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonProvider.DeserializeSimple<ApiResponse>(resultString);

            if (httpResponse.IsSuccessStatusCode && response?.Success == true)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, response.Role ?? "Estándar")
            };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = response?.Message ?? "Usuario o contraseña incorrectos.";
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.Error = "No se pudo conectar con el servidor. Intente más tarde.";
            return View(model);
        }
    }

    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            ViewBag.Error = "Las contraseñas no coinciden.";
            return View(model);
        }

        try
        {
            var json = JsonSerializer.Serialize(new
            {
                username = model.Username,
                password = model.Password
            });

            var result = await _restProvider.PostAsync($"{_apiBase}/api/account/register", json);
            var response = JsonProvider.DeserializeSimple<ApiResponse>(result);

            if (response?.Success == true)
                return RedirectToAction("Login");

            ViewBag.Error = response?.Message ?? "Error al registrar usuario.";
            return View(model);
        }
        catch (Exception)
        {
            ViewBag.Error = "No se pudo conectar con el servidor. Intente más tarde.";
            return View(model);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return View();
    }
}