using Architecture.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecetasHub.Models;
using System.Text.Json;

namespace RecetasHub.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly IRestProvider _restProvider;
    private readonly string _apiBase;

    public AdminController(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _apiBase = configuration["ApiSettings:BaseUrl"]!;
    }

    public IActionResult FuentesRecetas() => View();
    public IActionResult Secrets() => View();

    public async Task<IActionResult> GestionUsuarios()
    {
        try
        {
            var result = await _restProvider.GetAsync($"{_apiBase}/api/admin/users", null);
            var response = JsonProvider.DeserializeSimple<UsersApiResponse>(result);
            return View(response?.Data ?? new List<UserViewModel>());
        }
        catch (Exception)
        {
            ViewBag.Error = "No se pudo cargar la lista de usuarios.";
            return View(new List<UserViewModel>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(new
            {
                username = model.Username,
                password = model.Password,
                roleId = model.RoleId
            });

            var client = new HttpClient();
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_apiBase}/api/admin/users/create", content);
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonProvider.DeserializeSimple<ApiResponse>(resultString);

            TempData["Message"] = result?.Message;
            TempData["Success"] = response.IsSuccessStatusCode.ToString();
        }
        catch (Exception)
        {
            TempData["Message"] = "Error al crear el usuario.";
            TempData["Success"] = "False";
        }

        return RedirectToAction("GestionUsuarios");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(new
            {
                userId = model.UserId,
                newRoleId = model.NewRoleId
            });

            var client = new HttpClient();
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_apiBase}/api/admin/users/changerole", content);
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonProvider.DeserializeSimple<ApiResponse>(resultString);

            TempData["Message"] = result?.Message;
            TempData["Success"] = response.IsSuccessStatusCode.ToString();
        }
        catch (Exception)
        {
            TempData["Message"] = "Error al cambiar el rol.";
            TempData["Success"] = "False";
        }

        return RedirectToAction("GestionUsuarios");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            var client = new HttpClient();
            var response = await client.DeleteAsync($"{_apiBase}/api/admin/users/{userId}");
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonProvider.DeserializeSimple<ApiResponse>(resultString);

            TempData["Message"] = result?.Message;
            TempData["Success"] = response.IsSuccessStatusCode.ToString();
        }
        catch (Exception)
        {
            TempData["Message"] = "Error al eliminar el usuario.";
            TempData["Success"] = "False";
        }

        return RedirectToAction("GestionUsuarios");
    }
}