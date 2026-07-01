using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
