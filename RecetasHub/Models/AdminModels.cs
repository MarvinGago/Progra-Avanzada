namespace RecetasHub.Models;

public class UserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
}

public class UsersApiResponse
{
    public bool Success { get; set; }
    public List<UserViewModel> Data { get; set; } = new();
}

public class ChangeRoleViewModel
{
    public int UserId { get; set; }
    public int NewRoleId { get; set; }
}

public class RegisterAdminViewModel
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
public class CreateUserViewModel
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RoleId { get; set; }
}