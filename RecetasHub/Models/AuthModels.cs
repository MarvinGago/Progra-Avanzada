namespace RecetasHub.Models;

public class LoginViewModel
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterViewModel
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? Role { get; set; }
}