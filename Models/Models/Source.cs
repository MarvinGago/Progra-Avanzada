using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models;

public class Source
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string ComponentType { get; set; } = null!;
    public bool RequiresSecret { get; set; }
}