using Business;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var result = users.Select(u => new
            {
                id = u.Id,
                username = u.Username,
                roleId = u.RoleId,
                roleName = u.Role?.RoleName ?? "Sin rol"
            });
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("users/create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var message = await _userService.CreateUserAsync(request.Username, request.Password, request.RoleId);
            return Ok(new { success = true, message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("users/changerole")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleRequest request)
    {
        try
        {
            await _userService.ChangeRoleAsync(request.UserId, request.NewRoleId);
            return Ok(new { success = true, message = "Rol actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            await _userService.DeleteUserAsync(userId);
            return Ok(new { success = true, message = "Usuario eliminado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateUserRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RoleId { get; set; }
}

public class ChangeRoleRequest
{
    public int UserId { get; set; }
    public int NewRoleId { get; set; }
}