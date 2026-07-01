using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models;

public class SourceItem
{
    public int Id { get; set; }
    public int? SourceId { get; set; }
    public string? JsonData { get; set; }
    public DateTime CreatedAt { get; set; }

    public Source? Source { get; set; }
}