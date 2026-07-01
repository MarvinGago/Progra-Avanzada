using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class UserFavorite
{
    public int UserId { get; set; }

    public int SourceItemId { get; set; }

    public DateTime? SavedAt { get; set; }

    public virtual SourceItem SourceItem { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
