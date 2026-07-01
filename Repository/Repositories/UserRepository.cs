using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.Data.SqlClient;
using Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.Repositories;


public class UserRepository : IUserRepository
{
    private readonly RecetasHubContext _context;

    public UserRepository(RecetasHubContext context)
    {
        _context = context;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<(int ResultCode, string Message)> RegisterUserAsync(string username, string passwordHash, int roleId)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();
        using var command = connection.CreateCommand();
        command.CommandText = "sp_RegisterUser";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Username", username));
        command.Parameters.Add(new SqlParameter("@PasswordHash", passwordHash));
        command.Parameters.Add(new SqlParameter("@DefaultRoleId", roleId));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            int resultCode = reader.GetInt32(reader.GetOrdinal("ResultCode"));
            string message = reader.GetString(reader.GetOrdinal("Message"));
            return (resultCode, message);
        }

        return (-99, "No se recibió respuesta del procedimiento.");
    }

    public async Task<User?> LoginAsync(string username)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();
        using var command = connection.CreateCommand();
        command.CommandText = "sp_LoginUser";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@Username", username));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                Role = new Role
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    RoleName = reader.GetString(reader.GetOrdinal("RoleName"))
                }
            };
        }

        return null; // no existe el usuario
    }

    public async Task<IEnumerable<Models.Models.User>> GetAllUsersAsync()
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "sp_GetAllUsers";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        var users = new List<Models.Models.User>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new Models.Models.User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                RoleId = reader.GetInt32(reader.GetOrdinal("RoleId")),
                Role = new Models.Models.Role
                {
                    Id = reader.GetInt32(reader.GetOrdinal("RoleId")),
                    RoleName = reader.GetString(reader.GetOrdinal("RoleName"))
                }
            });
        }
        return users;
    }

    public async Task ChangeRoleAsync(int userId, int newRoleId)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "sp_ChangeUserRole";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@UserId", userId));
        command.Parameters.Add(new SqlParameter("@NewRoleId", newRoleId));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            int resultCode = reader.GetInt32(reader.GetOrdinal("ResultCode"));
            string message = reader.GetString(reader.GetOrdinal("Message"));
            if (resultCode != 1)
                throw new Exception(message);
        }
    }


    public async Task DeleteUserAsync(int userId)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "sp_DeleteUser";
        command.CommandType = System.Data.CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@UserId", userId));

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            int resultCode = reader.GetInt32(reader.GetOrdinal("ResultCode"));
            string message = reader.GetString(reader.GetOrdinal("Message"));
            if (resultCode != 1)
                throw new Exception(message);
        }
    }
}