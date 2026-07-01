using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models;

public class UserFavorite
{
    public int UserId { get; set; }
    public int SourceItemId { get; set; }
    public DateTime SavedAt { get; set; }

    public User? User { get; set; }
    public SourceItem? SourceItem { get; set; }
}
