using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class SourceItem
{
    public int Id { get; set; }

    public int? SourceId { get; set; }

    public string? JsonData { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Source? Source { get; set; }

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
