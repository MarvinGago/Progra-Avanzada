using System;
using System.Collections.Generic;

namespace Data.Entities;

public partial class SecretsSetting
{
    public int Id { get; set; }

    public string KeyName { get; set; } = null!;

    public string KeyValue { get; set; } = null!;

    public string? Description { get; set; }
}
