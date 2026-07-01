/*using System.Security.Cryptography;
using System.Text;
using Business;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

string password = "Admin123!";
using var sha256 = SHA256.Create();
var bytes = Encoding.UTF8.GetBytes(password);
var hashBytes = sha256.ComputeHash(bytes);
string hash = Convert.ToBase64String(hashBytes);
Console.WriteLine(hash);*/



using Business;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;

var options = new DbContextOptionsBuilder<RecetasHubContext>()
    .UseSqlServer("Server=.\\MSSQLSERVER02;Database=RecetasHub;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

var context = new RecetasHubContext(options);
var userRepo = new UserRepository(context);
var authService = new AuthService(userRepo);

Console.WriteLine("=== TEST 1: Registro de usuario nuevo ===");
try
{
    var result = await authService.RegisterAsync("chef_prueba", "Password123!");
    Console.WriteLine($"OK: {result}");
}
catch (BusinessException ex)
{
    Console.WriteLine($"ERROR de negocio: {ex.Message}");
}

Console.WriteLine("\n=== TEST 2: Registro con username duplicado ===");
try
{
    var result = await authService.RegisterAsync("chef_prueba", "Password123!");
    Console.WriteLine($"OK: {result}");
}
catch (BusinessException ex)
{
    Console.WriteLine($"ERROR de negocio (esperado): {ex.Message}");
}

Console.WriteLine("\n=== TEST 3: Registro con dominio restringido ===");
try
{
    var result = await authService.RegisterAsync("hacker@recetashub.com", "Password123!");
    Console.WriteLine($"OK: {result}");
}
catch (BusinessException ex)
{
    Console.WriteLine($"ERROR de negocio (esperado): {ex.Message}");
}

Console.WriteLine("\n=== TEST 4: Login correcto ===");
try
{
    var user = await authService.LoginAsync("chef_prueba", "Password123!");
    Console.WriteLine($"OK: Bienvenido {user.Username}, Rol: {user.Role?.RoleName}");
}
catch (BusinessException ex)
{
    Console.WriteLine($"ERROR de negocio: {ex.Message}");
}

Console.WriteLine("\n=== TEST 5: Login con contraseña incorrecta ===");
try
{
    var user = await authService.LoginAsync("chef_prueba", "ContraWrong!");
    Console.WriteLine($"OK: {user.Username}");
}
catch (BusinessException ex)
{
    Console.WriteLine($"ERROR de negocio (esperado): {ex.Message}");
}

Console.WriteLine("\n=== TEST 6: Login del admin inicial ===");
try
{
    var user = await authService.LoginAsync("admin_recetashub", "Admin123!");
    Console.WriteLine($"OK: Bienvenido {user.Username}, Rol: {user.Role?.RoleName}");
}
catch (BusinessException ex)
{
    Console.WriteLine($"ERROR de negocio: {ex.Message}");
}

Console.WriteLine("\nPresiona cualquier tecla para salir...");
Console.ReadKey();