using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models;

public class SecretsSetting
{
    public int Id { get; set; }
    public string KeyName { get; set; } = null!;
    public string KeyValue { get; set; } = null!;
    public string? Description { get; set; }
}