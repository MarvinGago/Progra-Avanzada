using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

namespace Business;

public interface IAuthService
{
    Task<string> RegisterAsync(string username, string password);
    Task<User> LoginAsync(string username, string password);
}